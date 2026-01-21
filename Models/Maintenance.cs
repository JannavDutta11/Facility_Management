namespace Facility_Management.Models
{
    public class Maintenance
    {
        public int MaintenanceId { get; set; }
        public int ResourceId { get; set; }
        public Resource? Resource { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}