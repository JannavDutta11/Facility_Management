//using Facility_Management.Models;
//using Microsoft.AspNetCore.Authorization;
//using Facility_Management.Dtos;
//using Microsoft.AspNetCore.Mvc;
//namespace Facility_Management.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class MaintenanceController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        public MaintenanceController(AppDbContext context)
//        {
//            _context = context;
//        }
//        // 1️⃣ SCHEDULE MAINTENANCE
//        [Authorize(Policy = "FacilityManagerOrAdmin")]
//        [HttpPost("schedule")]
//        public IActionResult ScheduleMaintenance(ScheduleMaintenanceDto dto)
//        {
//            var resource = _context.Resource.Find(dto.ResourceId);
//            if (resource == null)
//                return NotFound("Resource not found");
//            // Block resource
//            resource.IsAvailable = false;
//            resource.IsUnderMaintenance = true;
//            var maintenance = new Maintenance
//            {
//                ResourceId = dto.ResourceId,
//                MaintenanceType = dto.MaintenanceType,
//                StartDate = dto.StartDate,
//                EndDate = dto.EndDate,
//                Status = "Scheduled"
//            };
//            _context.Maintenances.Add(maintenance);
//            _context.SaveChanges();
//            return Ok("Maintenance scheduled and resource blocked");
//        }
//        // 2️⃣ COMPLETE MAINTENANCE + SAVE HISTORY
//        [Authorize(Policy = "FacilityManagerOrAdmin")]
//        [HttpPost("complete")]
//        public IActionResult CompleteMaintenance(MaintenanceHistoryDto dto)
//        {
//            var resource = _context.Resource.Find(dto.ResourceId);
//            if (resource == null)
//                return NotFound("Resource not found");
//            // Unblock resource
//            resource.IsAvailable = true;
//            var history = new MaintenanceHistory
//            {
//                ResourceId = dto.ResourceId,
//                WorkDone = dto.WorkDone,
//                Cost = dto.Cost,
//                TimeTakenHours = dto.TimeTakenHours,
//                PartsUsed = dto.PartsUsed,
//                CompletedOn = DateTime.Now
//            };
//            _context.MaintenanceHistories.Add(history);
//            // Update maintenance status
//            var maintenance = _context.Maintenances
//                .Where(m => m.ResourceId == dto.ResourceId && m.Status == "Scheduled")
//                .FirstOrDefault();
//            if (maintenance != null)
//                maintenance.Status = "Completed";
//            _context.SaveChanges();
//            return Ok("Maintenance completed and resource unblocked");
//        }
//        // 3️⃣ GET MAINTENANCE HISTORY
//        [Authorize(Policy = "FacilityManagerOrAdmin")]
//        [HttpGet("history/{resourceId}")]
//        public IActionResult GetMaintenanceHistory(int resourceId)
//        {
//            var history = _context.MaintenanceHistories
//                .Where(h => h.ResourceId == resourceId)
//                .ToList();
//            return Ok(history);
//        }
//    }
//}



using Facility_Management.Models;

using Microsoft.AspNetCore.Authorization;

using Facility_Management.Dtos;

using Microsoft.AspNetCore.Mvc;

using Facility_Management.Repository;

namespace Facility_Management.Controllers

{

    [ApiController]

    [Route("api/[controller]")]

    public class MaintenanceController : ControllerBase

    {

        private readonly AppDbContext _context;

        private readonly IResourceRepository _resourceRepo;

        public MaintenanceController(AppDbContext context, IResourceRepository resourceRepo)

        {

            _context = context;

            _resourceRepo = resourceRepo;

        }

        // 1️⃣ SCHEDULE MAINTENANCE

        [Authorize(Policy = "FacilityManagerOrAdmin")]

        [HttpPost("schedule")]

        public async Task<IActionResult> ScheduleMaintenance(ScheduleMaintenanceDto dto)

        {

            var resource = await _resourceRepo.GetByIdAsync(dto.ResourceId);

            if (resource == null)

                return NotFound("Resource not found");

            // ✅ Block resource via repository

            await _resourceRepo.SetMaintenanceStatusAsync(dto.ResourceId, true);

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

            return Ok("Maintenance scheduled and resource blocked");

        }

        // 2️⃣ COMPLETE MAINTENANCE + SAVE HISTORY

        [Authorize(Policy = "FacilityManagerOrAdmin")]

        [HttpPost("complete")]

        public async Task<IActionResult> CompleteMaintenance(MaintenanceHistoryDto dto)

        {

            var resource = await _resourceRepo.GetByIdAsync(dto.ResourceId);

            if (resource == null)

                return NotFound("Resource not found");

            // ✅ Unblock resource via repository

            await _resourceRepo.SetMaintenanceStatusAsync(dto.ResourceId, false);

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

            await _context.SaveChangesAsync();

            return Ok("Maintenance completed and resource unblocked");

        }

        // 3️⃣ GET MAINTENANCE HISTORY

        [Authorize(Policy = "FacilityManagerOrAdmin")]

        [HttpGet("history/{resourceId}")]

        public IActionResult GetMaintenanceHistory(int resourceId)

        {

            var history = _context.MaintenanceHistories

                .Where(h => h.ResourceId == resourceId)

                .ToList();

            return Ok(history);

        }

    }

}
