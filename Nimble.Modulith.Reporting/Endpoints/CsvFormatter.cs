using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Nimble.Modulith.Reporting.Endpoints;

public static class CsvFormatter
{
    public static string Format<T>(IEnumerable<T> data)
    {
        var sb = new StringBuilder();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Header
        sb.AppendLine(string.Join(",", props.Select(p => Escape(p.Name))));

        foreach (var item in data)
        {
            var values = props.Select(p =>
            {
                var value = p.GetValue(item);
                return Escape(value?.ToString() ?? string.Empty);
            });

            sb.AppendLine(string.Join(",", values));
        }

        return sb.ToString();
    }

    private static string Escape(string input)
    {
        if (input.Contains('"'))
            input = input.Replace("\"", "\"\"");

        if (input.Contains(',') || input.Contains('\n') || input.Contains('\r') || input.Contains('"'))
            input = $"\"{input}\"";

        return input;
    }

    public static bool WantsCsv(HttpRequest req, string? format)
    {
        if (!string.IsNullOrWhiteSpace(format) &&
            format.Equals("csv", StringComparison.OrdinalIgnoreCase))
            return true;

        if (req.Headers.TryGetValue("Accept", out var accept))
        {
            return accept.Any(x => x.Contains("text/csv", StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }
}