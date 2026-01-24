using Facility_Management.DTOs;
using Facility_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Facility_Management.Services
{
    public class AnalyticsService
    {
        private readonly AppDbContext _context;

        public AnalyticsService(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task<DashboardKpiDto> GetDashboardKpis()
        {
            var totalMaintenanceMinutes = await _context.Maintenances
                .SumAsync(x => EF.Functions.DateDiffMinute(x.StartDate, x.EndDate
));             

            return new DashboardKpiDto
            {
                TotalResources = await _context.Resource.CountAsync(),
                ActiveBookings = await _context.Bookings.CountAsync(x => x.Status == "Approved"),
                NoShowCount = await _context.Bookings.CountAsync(x => x.Status == "NoShow"),
                TotalMaintenanceMinutes = totalMaintenanceMinutes
            };
        }


        public async Task<IEnumerable<UtilizationDto>> GetUtilizationReport()
        {
            return await _context.Resource
                .Select(r => new UtilizationDto
                {
                    ResourceName = r.ResourceName,
                    TotalBookings = _context.Bookings.Count(b => b.ResourceId == r.Id),
                    TotalHoursUsed = _context.UsageLogs
                        .Where(u => u.ResourceId == r.Id)
                        .Sum(u => EF.Functions.DateDiffHour(u.StartTime, u.EndTime)),
                    UtilizationPercentage =
                        ((double)_context.UsageLogs
                        .Where(u => u.ResourceId == r.Id)
                        .Sum(u => EF.Functions.DateDiffMinute(u.StartTime, u.EndTime)) / (24 * 60)) * 100
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<UtilizationDto>> GetDailyUtilization()
        {
            return _context.Bookings
                .GroupBy(b => new { b.Resource.Id, b.Date.Date })
                .Select(g => new UtilizationDto
                {
                    Resource = g.First().Resource.ResourceName,
                    PlannedMinutes = g.Sum(x => x.PlannedMinutes),
                    ActualMinutes = g.Sum(x => x.ActualMinutes),
                    Utilization = (double)g.Sum(x => x.ActualMinutes) /
                                  g.Sum(x => x.PlannedMinutes) * 100
                })
                .ToList();
        }


        public async Task<IEnumerable<UtilizationDto>> GetWeeklyUtilization()
        {
            return _context.Bookings
                .GroupBy(b => new
                {
                    ResourceId = b.Resource.Id,
                    Week = EF.Functions.DateDiffWeek(DateTime.MinValue, b.Date)
                })
                .Select(g => new UtilizationDto
                {
                    Resource = g.First().Resource.ResourceName,
                    PlannedMinutes = g.Sum(x => x.PlannedMinutes),
                    ActualMinutes = g.Sum(x => x.ActualMinutes),
                    Utilization = (double)g.Sum(x => x.ActualMinutes) /
                                  g.Sum(x => x.PlannedMinutes) * 100
                })
                .ToList();
        }



        public async Task<IEnumerable<UtilizationDto>> GetMonthlyUtilization()
        {
            return _context.Bookings
                .GroupBy(b => new { b.Resource.Id, b.Date.Year, b.Date.Month })
                .Select(g => new UtilizationDto
                {
                    Resource = g.First().Resource.ResourceName,
                    PlannedMinutes = g.Sum(x => x.PlannedMinutes),
                    ActualMinutes = g.Sum(x => x.ActualMinutes),
                    Utilization = (double)g.Sum(x => x.ActualMinutes) /
                                  g.Sum(x => x.PlannedMinutes) * 100
                })
                .ToList();
        }




        public async Task<IEnumerable<PeakUsageDto>> GetPeakUsageReport()
        {
            return await _context.Resource.Select(r => new PeakUsageDto
            {
                ResourceName = r.ResourceName,
                PeakHour = _context.Bookings
                            .Where(b => b.ResourceId == r.Id)
                            .GroupBy(b => b.StartTime.Hour)
                            .OrderByDescending(g => g.Count())
                            .Select(g => g.Key)
                            .FirstOrDefault(),
                BookingCount = _context.Bookings.Count(b => b.ResourceId == r.Id)
            }).ToListAsync();
        }
    }
}
