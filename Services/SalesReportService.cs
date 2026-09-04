using System.Globalization;
using System.Text;

namespace ContosoPizza.Services;

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