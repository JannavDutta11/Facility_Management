namespace Facility_Management.Dtos
{
    public class ScheduleMaintenanceDto
    {
        public int ResourceId { get; set; }
        public string? MaintenanceType { get; set; } // Routine / Corrective
        public DateTime startDateTime { get; set; }
        public DateTime endDateTime { get; set; }
    }
}