using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Facility_Management.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AnalyticsService _analytics;

        public ReportsController(AnalyticsService analytics)
        {
            _analytics = analytics;
        }

        [HttpGet("kpis")]
        public async Task<IActionResult> GetDashboardKpis()
        {
            return Ok(await _analytics.GetDashboardKpis());
        }
        [HttpGet("utilization")]
        public async Task<IActionResult> GetUtilization()
        {
            return Ok(await _analytics.GetUtilizationReport());
        }
        [HttpGet("peak-usage")]
        public async Task<IActionResult> PeakUsage() =>

            Ok(await _analytics.GetPeakUsageReport());
    }
}
