using Facility_Management.DTOs;
using Facility_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;



namespace Facility_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // CREATE BOOKING
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
        {
            // 1️D Time validation
            if (dto.StartTime >= dto.EndTime)
                return BadRequest("StartTime must be before EndTime.");

            // 2️D Resource existence validation (Dev-1 integration)
            bool resourceExists = await _context.Resource
                .AnyAsync(r => r.ResourceId == dto.ResourceId);

            if (!resourceExists)
                return BadRequest("Invalid ResourceId. Resource does not exist.");
            var resource = await _context.Resource
        .FirstOrDefaultAsync(r => r.ResourceId == dto.ResourceId);
            if (resource == null)
                return BadRequest("Invalid ResourceId.");
            if (resource.IsUnderMaintenance)
                return BadRequest("Resource is under maintenance. Booking not allowed.");



            var rule = await _context.ResourceRule
    .FirstOrDefaultAsync(r => r.ResourceId == dto.ResourceId);

            if (rule == null)
                return BadRequest("No rules configured for this resource.");

            if (dto.StartTime.TimeOfDay < rule.StartTime.TimeOfDay || dto.EndTime.TimeOfDay > rule.EndTime.TimeOfDay)
                return BadRequest("Booking outside allowed hours.");

            var durationHours = (dto.EndTime - dto.StartTime).TotalHours;
            if (durationHours > rule.MaxBookingHours)
                return BadRequest("Booking exceeds maximum allowed duration.");


            var existingBookings = await _context.Bookings
    .Where(b =>
        b.ResourceId == dto.ResourceId &&
        b.Status == "Approved"
    )
    .ToListAsync(); //

            var bufferMinutes = rule.BufferMinutes;

            bool conflict = existingBookings.Any(b =>
            {
                var bufferedStart = b.StartTime.AddMinutes(-bufferMinutes);
                var bufferedEnd = b.EndTime.AddMinutes(bufferMinutes);

                return dto.StartTime < bufferedEnd &&
                       dto.EndTime > bufferedStart;
            });

            if (conflict)
                return BadRequest("Booking conflict detected (buffer time violation).");


            var booking = new Booking
            {
                ResourceId = dto.ResourceId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Purpose = dto.Purpose,
                NumberOfUsers = dto.NumberOfUsers,


                Status = rule.AutoApproveBooking ? "Approved" : "Pending",

                CreatedAt = DateTime.Now
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            return Ok(booking);


        
        }
        // APPROVE BOOKING
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            booking.Status = "Approved";
            await _context.SaveChangesAsync();

            return Ok(new { message ="Booking Approved" });
        }


        // REJECT BOOKING
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectBooking(int id, string reason)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            booking.Status = "Rejected";
            booking.RejectionReason = reason;

            await _context.SaveChangesAsync();
            return Ok(new { message="Booking Rejected" });
        }

        // CANCEL BOOKING
        [Authorize]
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
                return NotFound("Booking not found");

            if (booking.Status == "Cancelled")
                return BadRequest("Booking already cancelled");

            if (booking.Status == "Rejected" ||
                booking.Status == "Completed" ||
                booking.Status == "NoShow")
                return BadRequest($"Cannot cancel booking in {booking.Status} state");

            // Allowed only for Pending / Approved
            booking.Status = "Cancelled";
      

            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking cancelled successfully" });
        
        }


        // GET ALL BOOKINGS
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Bookings.ToListAsync();
            return Ok(bookings);
        }
    }
}