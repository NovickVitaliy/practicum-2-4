using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nimble.Modulith.Users.Data.Config;

public class IdentityRoleConfig : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        // Seed Admin and Customer roles
        builder.HasData(
            new IdentityRole
            {
                Id = "1E603235-6183-4E4D-8FF3-88768D8A9D80",
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "624221AF-31EA-489A-A8E8-7DA2C13877E4"
            },
            new IdentityRole
            {
                Id = "5BE9B034-D673-40F0-B59E-B6621D135979",
                Name = "Customer",
                NormalizedName = "CUSTOMER",
                ConcurrencyStamp = "E5FD9B42-11BA-4060-A862-2584BA5FD741"
            }
        );
    }
}