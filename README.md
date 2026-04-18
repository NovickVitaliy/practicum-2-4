# Nimble Modulith

> .NET 10 модульний моноліт на базі ASP.NET Core + Aspire

---

## Що це за проект?

**Nimble Modulith** — це навчально-демонстраційний проект, що реалізує архітектурний патерн **Modulith** (Modular Monolith): єдиний процес/деплоймент, але з чіткою модульною структурою всередині. Модулі спілкуються між собою виключно через контракти (Mediator-повідомлення), не маючи прямих залежностей один на одного.

### Технологічний стек

| Технологія | Призначення |
|---|---|
| .NET 10 / ASP.NET Core | Runtime та веб-фреймворк |
| .NET Aspire | Orchestration, service discovery, observability |
| FastEndpoints | Мінімалістичний роутинг ендпоінтів (замість Controllers) |
| Mediator (source-generated) | CQRS та inter-module communication |
| Entity Framework Core 10 | ORM для всіх модулів |
| SQL Server | База даних (окрема схема на кожен модуль) |
| Ardalis.Specification | Repository pattern + специфікації |
| Ardalis.Result | Типізовані результати операцій |
| MailKit / Papercut SMTP | Відправка email (локально через Papercut) |
| Serilog | Structured logging |
| JWT Bearer | Аутентифікація |

---

## Архітектура модулів

```
Nimble.Modulith.sln
│
├── _Host/
│   └── Nimble.Modulith.Web          ← Єдина точка входу (Program.cs)
│
├── Customers Module/
│   ├── Nimble.Modulith.Customers    ← Domain + Use Cases + Endpoints
│   └── Nimble.Modulith.Customers.Contracts  ← Публічні контракти (events, queries)
│
├── Products Module/
│   ├── Nimble.Modulith.Products     ← Domain + Endpoints
│   └── Nimble.Modulith.Products.Contracts   ← Публічні контракти
│
├── Users Module/
│   ├── Nimble.Modulith.Users        ← Identity + JWT + Endpoints
│   └── Nimble.Modulith.Users.Contracts      ← Публічні контракти
│
├── Nimble.Modulith.Email            ← Email відправка (background worker)
├── Nimble.Modulith.Email.Contracts  ← SendEmailCommand
├── Nimble.Modulith.Reporting        ← Star-schema аналітика
└── Nimble.Modulith.AppHost          ← Aspire AppHost
```

### Ізоляція модулів через схеми БД

Кожен модуль має власну SQL Server схему:

| Модуль | Схема | База даних |
|---|---|---|
| Users | `Users.*` | `usersdb` |
| Products | `Products.*` | `productsdb` |
| Customers | `Customers.*` | `customersdb` |
| Reporting | `Reporting.*` | `reportingdb` |

---

## Бізнес-процеси та флоу

### 1. Реєстрація та аутентифікація

```
Клієнт
  │
  ├─► POST /register-user  → UserManager.CreateAsync → Identity user в usersdb
  │
  ├─► POST /login-user     → SignInManager.PasswordSignInAsync → JWT токен
  │
  └─► POST /users/{id}/roles  (Admin only) → Додати роль Admin/Customer
                                           → Email нотифікація через Email модуль
```

**Ролі:**
- `Admin` — повний доступ до всіх ресурсів
- `Customer` — доступ лише до власних даних (перевірка по email)

---

### 2. Управління продуктами

```
POST /products      (Admin)   → Створити продукт (Name, Description, Price)
GET  /products                → Список всіх продуктів
GET  /products/{id}           → Отримати продукт за ID
PUT  /products/{id}  (Admin)  → Оновити продукт
DELETE /products/{id} (Admin) → Видалити продукт
```

Продукти зберігаються в схемі `Products.Products`. Ціна (`Price`) використовується при створенні замовлень.

---

### 3. Управління клієнтами

```
POST /customers          → Створити клієнта
                           ├─ Перевірка: існуючий Identity user?
                           │    └─ Ні → CreateUserCommand → Users модуль
                           │         → Тимчасовий пароль
                           └─ Email привітання
                           
GET  /customers           (Admin)         → Список всіх клієнтів
GET  /customers/{id}      (Admin/Owner)   → Деталі клієнта
```

**Authorization:** Admin або власник запису (email-match).

---

### 4. Замовлення — повний флоу

```
┌─────────────────────────────────────────────────────────┐
│                    ЗАМОВЛЕННЯ ФЛОУ                       │
└─────────────────────────────────────────────────────────┘

1. Створення замовлення
   POST /orders  { customerId, orderDate, items: [{productId, quantity}] }
     │
     ├─ Перевірка клієнта (GetCustomerByIdQuery → Customers модуль)
     ├─ Authorization: Admin або власник
     ├─ Для кожного товару → GetProductDetailsQuery → Products модуль
     │   (отримуємо назву та ціну)
     └─ Order зі статусом Pending зберігається в customersdb

2. Додавання товару до замовлення (статус Pending)
   POST /orders/{id}/items  { productId, quantity }
     └─ Якщо той самий productId → об'єднання кількостей (не дублювання)

3. Видалення товару з замовлення
   DELETE /orders/{id}/items/{itemId}

4. Підтвердження замовлення (КЛЮЧОВИЙ МОМЕНТ)
   POST /orders/{id}/confirm
     │
     ├─ Статус змінюється: Pending → Processing
     ├─ Публікується OrderCreatedEvent (Mediator notification)
     │     └─► Reporting модуль: OrderCreatedEventHandler
     │           ├─ Ідемпотентна перевірка (дублікати ігноруються)
     │           ├─ Upsert DimCustomer
     │           ├─ Upsert DimDate
     │           ├─ Upsert DimProduct
     │           └─ Insert FactOrder рядки (1 на кожен OrderItem)
     └─ Email підтвердження клієнту (SendEmailCommand → Email модуль)

Статуси замовлення:
  Pending → Confirmed → Processing → Shipped → Delivered
                                              → Cancelled
  
  ⚠️  Статус Confirmed = заморожено (не можна додавати/видаляти товари)
```

---

### 5. Email модуль (фоновий воркер)

```
SendEmailCommand (через Mediator)
  │
  └─► SendEmailCommandHandler
        └─► ChannelQueueService.EnqueueAsync (in-memory queue)
              │
              └─► EmailSendingBackgroundWorker (HostedService, кожну ~1 сек)
                    └─► SmtpEmailSender (MailKit → Papercut SMTP :25)
```

Email НЕ відправляється синхронно — команда лише ставить повідомлення в чергу.

---

### 6. Reporting модуль (Star Schema)

```
Схема даних:
  DimCustomer ──┐
  DimDate ──────┼── FactOrder (по 1 рядку на OrderItem)
  DimProduct ───┘

Ендпоінти (Admin only):
  GET /reports/orders?StartDate=&EndDate=           → Звіт по замовленнях
  GET /reports/product-sales?StartDate=&EndDate=    → Продажі по продуктах
  GET /reports/customers/{id}/orders                → Замовлення клієнта

  Формат відповіді: JSON (default) або CSV (?Format=csv або Accept: text/csv)
```

**Важливо:** дані в Reporting з'являються ТІЛЬКИ після `POST /orders/{id}/confirm`.

---

## Inter-module Communication

Модулі спілкуються виключно через Mediator повідомлення:

| Команда/Запит | Від | До |
|---|---|---|
| `CreateUserCommand` | Customers | Users |
| `GetProductDetailsQuery` | Customers | Products |
| `GetProductPriceQuery` | Customers | Products |
| `SendEmailCommand` | Users, Customers | Email |
| `OrderCreatedEvent` (INotification) | Customers | Reporting |

Жодних прямих проектних посилань між модулями (крім `.Contracts`-проектів).

---

## Запуск проекту

### Вимоги
- .NET 10 SDK
- Docker Desktop (для SQL Server та Papercut через Aspire)

### Запуск
```bash
cd Nimble.Modulith.AppHost
dotnet run
```

Aspire Dashboard відкриється автоматично (~`https://localhost:17143`).

Swagger UI: `http://localhost:{webapi-port}/swagger`

### Локальний SMTP (Papercut)
Papercut UI для перегляду листів: `http://localhost:37408`

---

## Конфігурація JWT

У `appsettings.json`:
```json
{
  "Auth": {
    "JwtSecret": "D4165832-5F86-445C-9550-5EF2AFF49828"
  }
}
```

При логіні через `/login-user` отримуєте JWT токен. Передавайте у заголовку:
```
Authorization: Bearer <token>
```

---

## Структура даних Reporting

```sql
-- DimDate: попередньо заповнено на 2025 рік (365 рядків)
-- В Development: БД перестворюється при кожному запуску

DimCustomer (CustomerId PK, Email)
DimProduct  (ProductId PK, ProductName)
DimDate     (DateKey PK, Date, Year, Quarter, Month, Day, ...)
FactOrder   (OrderNumber + OrderItemId PK,
             DimCustomerId FK, DimDateId FK, DimProductId FK,
             Quantity, UnitPrice, TotalPrice, OrderTotalAmount)
```
