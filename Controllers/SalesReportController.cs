using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("sales")]
public class SalesReportController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] SalesReportRequest request)
    {
        if (!Directory.Exists(request.SalesDirectory))
            return BadRequest($"Sales directory does not exist: {request.SalesDirectory}");

        if (string.IsNullOrWhiteSpace(request.ReportFilePath))
            return BadRequest("Report file path is required.");

        var reportPath = SalesReportService.GenerateSalesSummaryReport(
            request.SalesDirectory,
            request.ReportFilePath);

        return Ok(new { reportPath });
    }
}

public sealed class SalesReportRequest
{
    public string SalesDirectory { get; set; } = string.Empty;
    public string ReportFilePath { get; set; } = string.Empty;
}