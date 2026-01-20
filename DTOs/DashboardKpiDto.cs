namespace Facility_Management.DTOs
{
    public class DashboardKpiDto
    {
        public int TotalResources { get; set; }
        public int ActiveBookings { get; set; }
        public int NoShowCount { get; set; }
        public int TotalMaintenanceMinutes { get; set; }
    }
}
