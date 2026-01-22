namespace Facility_Management.DTOs
{
    public class MaintenanceCreateDto
    {
        public int ResourceId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}