namespace Facility_Management.DTOs
{
    public class UtilizationDto
    {
        public string ResourceName { get; set; }
        public double UtilizationPercentage { get; set; }
        public int TotalBookings { get; set; }
        public double TotalHoursUsed { get; set; }
    }
}
