using Facility_Management.DTOs;
using Facility_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Facility_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MaintenanceController(AppDbContext context)
        {
            _context = context;
        }
        // POST: api/Maintenance
        [HttpPost]
        public IActionResult ScheduleMaintenance(MaintenanceCreateDto dto)
        {
            if (dto.EndDate <= dto.StartDate)
            {
                return BadRequest("End date must be greater than start date");
            }
            var resource = _context.Resource.Find(dto.ResourceId);
            if (resource == null)
            {
                return NotFound("Resource not found");
            }
            var maintenance = new Maintenance
            {
                ResourceId = dto.ResourceId,
                Reason = dto.Reason,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsCompleted = false
            };
            _context.Maintenances.Add(maintenance);
            _context.SaveChanges();
            return Ok(maintenance);
        }
        // GET: api/Maintenance
        [HttpGet]
        public IActionResult GetAllMaintenances()
        {
            var data = _context.Maintenances
                .Include(m => m.Resource)
                .ToList();
            return Ok(data);
        }
        // PUT: api/Maintenance/complete/5
        [HttpPut("complete/{id}")]
        public IActionResult CompleteMaintenance(int id)
        {
            var maintenance = _context.Maintenances.Find(id);
            if (maintenance == null)
            {
                return NotFound();
            }
            maintenance.IsCompleted = true;
            _context.SaveChanges();
            return Ok(maintenance);
        }
    }
}