using Facility_Management.DTOs;
using Facility_Management.Models;
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
         
            [HttpPost("create")]
            public async Task<IActionResult> CreateBooking(BookingDto dto)
            {
                if (dto.StartTime >= dto.EndTime)
                    return BadRequest("StartTime must be before EndTime.");

                if(dto.NumberOfUsers <=0)
                return BadRequest("Number of users must be greater than zero");

            bool resourceExists = await _context.Resource.AnyAsync(r => r.ResourceId == dto.ResourceId);

            if (!resourceExists)
                return BadRequest("Invalid ResourceId. Resource does not exist.");



            bool conflict = await _context.Bookings.AnyAsync(b =>
                    b.ResourceId == dto.ResourceId &&
                    b.Status == "Approved" &&
                    dto.StartTime < b.EndTime &&
                    dto.EndTime > b.StartTime
                );

                if (conflict)
                    return BadRequest("Booking conflict detected.");




            Booking booking = new Booking
            {
                ResourceId = dto.ResourceId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Purpose = dto.Purpose,
                NumberOfUsers = dto.NumberOfUsers,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            return Ok(booking);
        }

        // APPROVE BOOKING

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
           
            [HttpGet("all")]
            public async Task<IActionResult> GetAllBookings()
            {
                var bookings = await _context.Bookings.ToListAsync();
                return Ok(bookings);
            }
        }
    }



