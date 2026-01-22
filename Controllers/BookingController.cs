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
            
        public async Task<IActionResult> CreateBooking(BookingDto dto)
            {
            
            if (dto.StartTime >= dto.EndTime)
                return BadRequest("StartTime must be before EndTime.");

            
            bool resourceExists = await _context.Resource
                .AnyAsync(r => r.ResourceId == dto.ResourceId);

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

                return Ok("Booking Approved");
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
                return Ok("Booking Rejected");
            }

        // CANCEL BOOKING

        [Authorize]
        [HttpPut("cancel/{id}")]
            
        public async Task<IActionResult> CancelBooking(int id)
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                    return NotFound();

                booking.Status = "Cancelled";
                await _context.SaveChangesAsync();

                return Ok("Booking Cancelled");
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



