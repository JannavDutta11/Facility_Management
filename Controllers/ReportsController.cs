using Facility_Management.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Facility_Management.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [EnableCors("AllowAngular")]
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
            var result = await _analytics.GetDashboardKpis();
            return Ok(result);
        }

        [HttpGet("utilization")]
        public async Task<IActionResult> GetUtilization()
        {
            var result = await _analytics.GetUtilizationReport();
            return Ok(result);
        }

      
        [HttpGet("peak-usage")]
        public async Task<IActionResult> GetPeakUsage()
        {
            var result = await _analytics.GetPeakUsageReport();
            return Ok(result);
        }
    }
}
