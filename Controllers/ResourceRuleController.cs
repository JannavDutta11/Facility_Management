using Facility_Management.DTOs;
using Facility_Management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Facility_Management.Controllers

    {
        [ApiController]
        [Route("api/resourcerules")]
        public class ResourceRuleController : ControllerBase
        {
            private readonly AppDbContext _context;

            public ResourceRuleController(AppDbContext context)
            {
                _context = context;
            }
            [HttpPost]
            public async Task<IActionResult> Create(CreateResourceRuleDto dto)
            {
                if (dto.StartTime >= dto.EndTime)
                    return BadRequest("Invalid working hours");

                var rule = new ResourceRule
                {
                    ResourceId = dto.ResourceId,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    MaxBookingHours = dto.MaxBookingHours,
                    BufferMinutes = dto.BufferMinutes,
                    AutoApproveBooking = dto.AutoApproveBooking,
                    AdminApprovalRequired = dto.AdminApprovalRequired
                };

                _context.ResourceRule.Add(rule);
                await _context.SaveChangesAsync();

            return Ok(rule);
        }

        [HttpGet("{resourceId}")]
            public async Task<IActionResult> GetByResource(int resourceId)
            {
                var rule = await _context.ResourceRule.FirstOrDefaultAsync(r => r.ResourceId == resourceId);

                if (rule == null)
                    return NotFound();

                return Ok(rule);
            }
        }
    }

    




