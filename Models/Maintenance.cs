using System.ComponentModel.DataAnnotations;
namespace Facility_Management.Models
{
    public class Maintenance
    {
        [Key]
        public int MaintenanceId { get; set; }
        public int ResourceId { get; set; }
        public string? MaintenanceType { get; set; } // Routine / Corrective
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; } // Scheduled / Completed
    }
}