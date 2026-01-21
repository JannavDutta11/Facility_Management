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
                .SumAsync(x => EF.Functions.DateDiffMinute(x.StartTime, x.EndTime));

            return new DashboardKpiDto
            {
                TotalResources = await _context.Resources.CountAsync(),
                ActiveBookings = await _context.Bookings.CountAsync(x => x.Status == "Approved"),
                NoShowCount = await _context.Bookings.CountAsync(x => x.Status == "NoShow"),
                TotalMaintenanceMinutes = totalMaintenanceMinutes
            };
        }


        public async Task<IEnumerable<UtilizationDto>> GetUtilizationReport()
        {
            return await _context.Resources
                .Select(r => new UtilizationDto
                {
                    ResourceName = r.Name,
                    TotalBookings = _context.Bookings.Count(b => b.ResourceId == r.Id),
                    TotalHoursUsed = _context.Usages
                        .Where(u => u.ResourceId == r.Id)
                        .Sum(u => EF.Functions.DateDiffHour(u.StartTime, u.EndTime)),
                    UtilizationPercentage =
                        ((double)_context.Usages
                        .Where(u => u.ResourceId == r.Id)
                        .Sum(u => EF.Functions.DateDiffMinute(u.StartTime, u.EndTime)) / (24 * 60)) * 100
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<PeakUsageDto>> GetPeakUsageReport()
        {
            return await _context.Resources.Select(r => new PeakUsageDto
            {
                ResourceName = r.Name,
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



 

