using System.ComponentModel.DataAnnotations;
namespace Facility_Management.Models
{
    public class MaintenanceHistory
    {
        [Key]
        public int HistoryId { get; set; }
        public int ResourceId { get; set; }
        public string WorkDone { get; set; }
        public int Cost { get; set; }
        public int TimeTakenHours { get; set; }
        public string PartsUsed { get; set; }
        public DateTime CompletedOn { get; set; }
    }
}