using Facility_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Facility_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MaintenanceController(AppDbContext context)
        {
            _context = context;
        }
        // ===============================
        // 1. CREATE MAINTENANCE
        // ===============================
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpPost]
        
        public async Task<IActionResult> CreateMaintenance(Maintenance maintenance)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _context.Maintenances.Add(maintenance);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMaintenanceById),
                new { id = maintenance.MaintenanceId }, maintenance);
        }
        // ===============================
        // 2. GET ALL MAINTENANCE
        // ===============================
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpGet]
        

        public async Task<IActionResult> GetAllMaintenance()
        {
            var data = await _context.Maintenances.ToListAsync();
            return Ok(data);
        }
        // ===============================
        // 3. GET MAINTENANCE BY ID
        // ===============================
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpGet("{id}")]
        
        public async Task<IActionResult> GetMaintenanceById(int id)
        {
            var maintenance = await _context.Maintenances.FindAsync(id);
            if (maintenance == null)
                return NotFound();
            return Ok(maintenance);
        }
        // ===============================
        // 4. GET MAINTENANCE BY RESOURCE
        // ===============================
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpGet("resource/{resourceId}")]
        
        public async Task<IActionResult> GetByResourceId(int resourceId)
        {
            var data = await _context.Maintenances
                .Where(m => m.ResourceId == resourceId)
                .ToListAsync();
            return Ok(data);
        }
        // ===============================
        // 5. UPDATE MAINTENANCE
        // ===============================
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpPut("{id}")]
        
        public async Task<IActionResult> UpdateMaintenance(int id, Maintenance maintenance)
        {
            if (id != maintenance.MaintenanceId)
                return BadRequest("Maintenance ID mismatch");
            _context.Entry(maintenance).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(maintenance);
        }
        // ===============================
        // 6. DELETE / CLOSE MAINTENANCE
        // ===============================
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpDelete("{id}")]
        
        public async Task<IActionResult> DeleteMaintenance(int id)
        {
            var maintenance = await _context.Maintenances.FindAsync(id);
            if (maintenance == null)
                return NotFound();
            _context.Maintenances.Remove(maintenance);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Maintenance record removed" });
        }
    }
}