using System;
using System.Text.Json;
using System.Threading.Tasks;
using Facility_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace Facility_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsageController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        public UsageController(AppDbContext ctx) => _ctx = ctx;

        
        [Authorize] 
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
        {
            var booking = await _ctx.Bookings.FindAsync(dto.BookingId);
            if (booking == null) return NotFound("Booking not found.");
            if (!string.Equals(booking.Status, "Approved", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only approved bookings can be checked in.");

            var usage = await _ctx.UsageLogs.FirstOrDefaultAsync(u => u.BookingId == dto.BookingId);
            if (usage == null)
            {
                usage = new UsageLog
                {
                    BookingId = dto.BookingId,
                    ActualStartTime = DateTime.Now,
                    Status = "CheckedIn",
                    Source = "User"
                };
                await _ctx.UsageLogs.AddAsync(usage);
            }
            else
            {
                usage.ActualStartTime ??= DateTime.Now;
                usage.Status = "CheckedIn";
                usage.Source = "User";
                _ctx.UsageLogs.Update(usage);
            }

           
            await _ctx.SaveChangesAsync();
            return Ok(new { Message = "Checked-in", usage.UsageLogId, usage.ActualStartTime });
        }

       
        [Authorize]
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutDto dto)
        {
            var usage = await _ctx.UsageLogs.FirstOrDefaultAsync(u => u.BookingId == dto.BookingId);
            if (usage == null || usage.ActualStartTime == null)
                return BadRequest("Check-in is required before check-out.");

            var end = DateTime.Now;
            if (end < usage.ActualStartTime.Value) return BadRequest("End cannot be before start.");

            usage.ActualEndTime = end;
            usage.ActualCapacityUsed = dto.ActualCapacityUsed;
            usage.Status = "CheckedOut";
            usage.Source = "User";

            var booking = await _ctx.Bookings.FindAsync(dto.BookingId);
            if (booking != null) booking.Status = "Completed";

            await _ctx.SaveChangesAsync();
            return Ok(new { Message = "Checked-out", usage.ActualEndTime, usage.ActualCapacityUsed });
        }

       
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpPost("admin/checkin")]
        public async Task<IActionResult> AdminCheckIn([FromBody] AdminCheckInDto dto)
        {
            var booking = await _ctx.Bookings.FindAsync(dto.BookingId);
            if (booking == null) return NotFound("Booking not found.");
            if (!string.Equals(booking.Status, "Approved", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only approved bookings can be checked in.");

            var start = dto.ActualStartTime ?? DateTime.Now;

            var usage = await _ctx.UsageLogs.FirstOrDefaultAsync(u => u.BookingId == dto.BookingId);
            var old = usage == null ? null : new { usage.ActualStartTime, usage.Status, usage.Source };

            if (usage == null)
            {
                usage = new UsageLog
                {
                    BookingId = dto.BookingId,
                    ActualStartTime = start,
                    Status = "CheckedIn",
                    Source = "Admin"
                };
                await _ctx.UsageLogs.AddAsync(usage);
            }
            else
            {
                usage.ActualStartTime = start;
                usage.Status = "CheckedIn";
                usage.Source = "Admin";
                _ctx.UsageLogs.Update(usage);
            }

            
            await _ctx.SaveChangesAsync();
            await WriteAudit(booking.BookingId, usage.UsageLogId, "AdminCheckIn", dto.Reason, old, usage);
            return Ok(new { Message = "Admin check-in recorded", usage.UsageLogId, usage.ActualStartTime });
        }

       [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpPost("admin/checkout")]
        public async Task<IActionResult> AdminCheckOut([FromBody] AdminCheckOutDto dto)
        {
            var booking = await _ctx.Bookings.FindAsync(dto.BookingId);
            if (booking == null) return NotFound("Booking not found.");

            var usage = await _ctx.UsageLogs.FirstOrDefaultAsync(u => u.BookingId == dto.BookingId);
            if (usage == null || usage.ActualStartTime == null)
                return BadRequest("Cannot check-out before check-in.");

            var end = dto.ActualEndTime ?? DateTime.Now;
            if (end < usage.ActualStartTime.Value) return BadRequest("End cannot be before start.");

            var old = new { usage.ActualEndTime, usage.ActualCapacityUsed, usage.Status, usage.Source };

            usage.ActualEndTime = end;
            usage.ActualCapacityUsed = dto.ActualCapacityUsed;
            usage.Status = "CheckedOut";
            usage.Source = "Admin";

            booking.Status = "Completed";

            await _ctx.SaveChangesAsync();
            await WriteAudit(booking.BookingId, usage.UsageLogId, "AdminCheckOut", dto.Reason, old, usage);

            return Ok(new { Message = "Admin check-out recorded", usage.ActualEndTime, usage.ActualCapacityUsed });
        }

       
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpPost("admin/backfill")]
        public async Task<IActionResult> AdminBackfill([FromBody] AdminBackfillDto dto)
        {
            if (dto.ActualEndTime <= dto.ActualStartTime)
                return BadRequest("Start must be before end.");

            var booking = await _ctx.Bookings.FindAsync(dto.BookingId);
            if (booking == null) return NotFound("Booking not found.");

            if (dto.EnforceWithinBookingWindow)
            {
                if (dto.ActualStartTime < booking.StartTime || dto.ActualEndTime > booking.EndTime)
                    return BadRequest("Usage must fall within booking window.");
            }

            var usage = await _ctx.UsageLogs.FirstOrDefaultAsync(u => u.BookingId == dto.BookingId);
            var old = usage == null ? null : new { usage.ActualStartTime, usage.ActualEndTime, usage.ActualCapacityUsed, usage.Status, usage.Source };

            if (usage == null)
            {
                usage = new UsageLog
                {
                    BookingId = dto.BookingId,
                    ActualStartTime = dto.ActualStartTime,
                    ActualEndTime = dto.ActualEndTime,
                    ActualCapacityUsed = dto.ActualCapacityUsed,
                    Status = "CheckedOut",
                    Source = "Admin"
                };
                await _ctx.UsageLogs.AddAsync(usage);
            }
            else
            {
                usage.ActualStartTime = dto.ActualStartTime;
                usage.ActualEndTime = dto.ActualEndTime;
                usage.ActualCapacityUsed = dto.ActualCapacityUsed;
                usage.Status = "CheckedOut";
                usage.Source = "Admin";
                _ctx.UsageLogs.Update(usage);
            }

            booking.Status = "Completed";

            await _ctx.SaveChangesAsync();
            await WriteAudit(booking.BookingId, usage.UsageLogId, "AdminBackfill", dto.Reason, old, usage);
            return Ok(new { Message = "Backfilled usage saved." });
        }

       
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpPatch("admin/edit")]
        public async Task<IActionResult> AdminEdit([FromBody] AdminEditUsageDto dto)
        {
            var usage = await _ctx.UsageLogs.FirstOrDefaultAsync(u => u.BookingId == dto.BookingId);
            if (usage == null) return NotFound("Usage not found.");

            var old = new { usage.ActualStartTime, usage.ActualEndTime, usage.ActualCapacityUsed, usage.Status, usage.Source };

            if (dto.ActualStartTime.HasValue) usage.ActualStartTime = dto.ActualStartTime;
            if (dto.ActualEndTime.HasValue) usage.ActualEndTime = dto.ActualEndTime;
            if (dto.ActualCapacityUsed.HasValue) usage.ActualCapacityUsed = dto.ActualCapacityUsed;

            if (usage.ActualStartTime != null && usage.ActualEndTime != null &&
                usage.ActualEndTime < usage.ActualStartTime)
                return BadRequest("End cannot be before start.");

            usage.Status = "ManuallyUpdated";
            usage.Source = "Admin";

            var booking = await _ctx.Bookings.FindAsync(dto.BookingId);
            if (booking != null && usage.ActualEndTime != null)
                booking.Status = "Completed";

            await _ctx.SaveChangesAsync();
            await WriteAudit(dto.BookingId, usage.UsageLogId, "AdminEdit", dto.Reason, old, usage);
            return Ok(new { Message = "Usage updated by admin." });
        }

       
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("admin/mark-noshow/{bookingId:int}")]
        public async Task<IActionResult> MarkNoShow([FromRoute] int bookingId, [FromBody] string? reason)
        {
            var booking = await _ctx.Bookings.FindAsync(bookingId);
            if (booking == null) return NotFound("Booking not found.");
            if (string.Equals(booking.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Completed bookings cannot be marked as No-Show.");

            booking.Status = "NoShow";
            await _ctx.UsageLogs.AddAsync(new UsageLog { BookingId = bookingId, Status = "NoShow", Source = "Admin" });
            await _ctx.SaveChangesAsync();

            await WriteAudit(bookingId, null, "MarkNoShow", reason, null, new { BookingStatus = booking.Status });
            return Ok(new { Message = "Marked as No-Show." });
        }

       
        [Authorize]
        [HttpGet("utilization/{bookingId:int}")]
        public async Task<IActionResult> GetUtilization([FromRoute] int bookingId)
        {
            var booking = await _ctx.Bookings.FindAsync(bookingId);
            if (booking == null) return NotFound("Booking not found.");

            var usage = await _ctx.UsageLogs.FirstOrDefaultAsync(u => u.BookingId == bookingId);
            if (usage == null || usage.ActualStartTime == null || usage.ActualEndTime == null)
                return NotFound("Usage not available yet.");

            var planned = (booking.EndTime - booking.StartTime).TotalMinutes;
            var actual = (usage.ActualEndTime.Value - usage.ActualStartTime.Value).TotalMinutes;
            var utilization = planned <= 0 ? 0 : Math.Round((actual / planned) * 100.0, 2);

            return Ok(new
            {
                BookingId = bookingId,
                PlannedMinutes = planned,
                ActualMinutes = actual,
                UtilizationPercent = utilization,
                BookingStatus = booking.Status,
                UsageSource = usage.Source
            });
        }

       
        private async Task WriteAudit(int bookingId, int? usageLogId, string action, string? reason, object? oldObj, object? newObj)
        {
            try
            {
                var changedBy = User?.Identity?.Name ?? "system";
                var audit = new UsageAudit
                {
                    BookingId = bookingId,
                    UsageLogId = usageLogId,
                    Action = action,
                    ChangedBy = changedBy,
                    Reason = reason,
                    DiffJson = JsonSerializer.Serialize(new { Old = oldObj, New = newObj })
                };
                _ctx.UsageAudits.Add(audit);
                await _ctx.SaveChangesAsync();
            }
            catch { }
        }
    }
}





