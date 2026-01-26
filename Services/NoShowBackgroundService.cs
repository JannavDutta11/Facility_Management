
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Facility_Management.Models
{
    public class NoShowBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<NoShowBackgroundService> _logger;
        private readonly int _intervalSeconds;
        private readonly int _graceMinutes;

        public NoShowBackgroundService(IServiceProvider sp, ILogger<NoShowBackgroundService> logger, IConfiguration cfg)
        {
            _sp = sp; _logger = logger;
            _intervalSeconds = cfg.GetValue<int>("NoShow:CheckIntervalSeconds", 60);
            _graceMinutes = cfg.GetValue<int>("NoShow:GraceMinutes", 10);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_intervalSeconds), stoppingToken);
                    await CheckNoShowsAsync(stoppingToken);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex) { _logger.LogError(ex, "NoShow worker error"); }
            }
        }

        private async Task CheckNoShowsAsync(CancellationToken ct)
        {
            using var scope = _sp.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var now = DateTime.Now;

            var due = await ctx.Bookings
                .Where(b => b.Status == "Approved"
                            && now > b.StartTime.AddMinutes(_graceMinutes)
                            && !ctx.UsageLogs.Any(u => u.BookingId == b.BookingId && u.ActualStartTime != null))
                .ToListAsync(ct);

            if (!due.Any()) return;

            foreach (var booking in due)
            {
                booking.Status = "NoShow";
                ctx.UsageLogs.Add(new UsageLog { BookingId = booking.BookingId, Status = "NoShow", Source = "System" });
            }
            await ctx.SaveChangesAsync(ct);
        }
    }
}

