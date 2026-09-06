http://localhost:5184/> get pizza
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 04 Sep 2026 15:14:08 GMT
Server: Kestrel
Transfer-Encoding: chunked

[
  {
    "id": 1,
    "name": "Classic Italian",
    "isGlutenFree": false
  },
  {
    "id": 2,
    "name": "Veggie",
    "isGlutenFree": true
  },
  {
    "id": 4,
    "name": "Hawaiian",
    "isGlutenFree": false
  },
  {
    "id": 5,
    "name": "Pepperoni",
    "isGlutenFree": false
  },
  {
    "id": 6,
    "name": "BBQ Chicken",
    "isGlutenFree": false
  },
  {
    "id": 7,
    "name": "Mexican",
    "isGlutenFree": false
  }
]

Sales Summary
----------------------------
 Total Sales: $1,001.24

 Details:
  store-a.txt: $425.49
  store-b.txt: $575.75

**Sales Summary Functions**

public static class SalesReportService
{
    public static string GenerateSalesSummaryReport(string salesDirectory, string reportFilePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(salesDirectory);
        ArgumentException.ThrowIfNullOrEmpty(reportFilePath);

        if (!Directory.Exists(salesDirectory))
            throw new DirectoryNotFoundException($"Sales directory not found: {salesDirectory}");

        var reportPath = Path.GetFullPath(reportFilePath);
        var salesByFile = Directory.EnumerateFiles(salesDirectory)
            .Where(filePath => !string.Equals(
                Path.GetFullPath(filePath),
                reportPath,
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(filePath => filePath, StringComparer.OrdinalIgnoreCase)
            .Select(filePath => new
            {
                FileName = Path.GetFileName(filePath),
                Total = CalculateFileTotal(filePath)
            })
            .ToList();

        var totalSales = salesByFile.Sum(sale => sale.Total);
        var report = new StringBuilder()
            .AppendLine("Sales Summary")
            .AppendLine("----------------------------")
            .AppendLine($" Total Sales: {totalSales.ToString("C", CultureInfo.GetCultureInfo("en-US"))}")
            .AppendLine()
            .AppendLine(" Details:");

        foreach (var sale in salesByFile)
        {
            report.AppendLine($"  {sale.FileName}: {sale.Total.ToString("C", CultureInfo.GetCultureInfo("en-US"))}");
        }

        var reportDirectory = Path.GetDirectoryName(reportPath);
        if (!string.IsNullOrEmpty(reportDirectory))
            Directory.CreateDirectory(reportDirectory);

        File.WriteAllText(reportPath, report.ToString());
        return reportPath;
    }

    private static decimal CalculateFileTotal(string filePath)
    {
        decimal total = 0;

        foreach (var line in File.ReadLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (!decimal.TryParse(line.Trim(), NumberStyles.Currency, CultureInfo.InvariantCulture, out var sale))
                throw new FormatException($"Sales file contains an invalid amount: {filePath}");

            total += sale;
        }

        return total;
    }
}