using Facility_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        // ===============================
        // 1. CREATE MAINTENANCE
        // ===============================
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpPost]
        
        public async Task<IActionResult> CreateMaintenance(Maintenance maintenance)
        {
            var resource = _context.Resource.Find(dto.ResourceId);
            if (resource == null)
                return NotFound("Resource not found");
            // Block resource
            resource.IsAvailable = false;
            var maintenance = new Maintenance
            {
                ResourceId = dto.ResourceId,
                MaintenanceType = dto.MaintenanceType,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "Scheduled"
            };
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
            var resource = _context.Resource.Find(dto.ResourceId);
            if (resource == null)
                return NotFound("Resource not found");
            // Unblock resource
            resource.IsAvailable = true;
            var history = new MaintenanceHistory
            {
                ResourceId = dto.ResourceId,
                WorkDone = dto.WorkDone,
                Cost = dto.Cost,
                TimeTakenHours = dto.TimeTakenHours,
                PartsUsed = dto.PartsUsed,
                CompletedOn = DateTime.Now
            };
            _context.MaintenanceHistories.Add(history);
            // Update maintenance status
            var maintenance = _context.Maintenances
                .Where(m => m.ResourceId == dto.ResourceId && m.Status == "Scheduled")
                .FirstOrDefault();
            if (maintenance != null)
                maintenance.Status = "Completed";
            _context.SaveChanges();
            return Ok("Maintenance completed and resource unblocked");
        }
        // ===============================
        // 6. DELETE / CLOSE MAINTENANCE
        // ===============================
        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpDelete("{id}")]
        
        public async Task<IActionResult> DeleteMaintenance(int id)
        {
            var history = _context.MaintenanceHistories
                .Where(h => h.ResourceId == resourceId)
                .ToList();
            return Ok(history);
        }
    }
}