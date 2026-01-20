using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Facility_Management.Models
{
    public class Maintenance
    {
        [Key]
        public int MaintenanceId { get; set; }
        [Required]
        public int ResourceId { get; set; }
        [Required]
        public string MaintenanceType { get; set; } = null!;
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;
        // Navigation Property
        [ForeignKey("ResourceId")]
        public Resource? Resource { get; set; }
    }
}