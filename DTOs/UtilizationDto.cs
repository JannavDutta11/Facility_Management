namespace Facility_Management.DTOs
{
    public class UtilizationDto
    {
        public string ResourceName { get; set; }
        public double UtilizationPercentage { get; set; }

        public string Resource { get; set; }

        public double Utilization {  get; set; }
        public int TotalBookings { get; set; }
        public double TotalHoursUsed { get; set; }
        public int BookingId { get; internal set; }
        public int ResourceId { get; internal set; }
        public double PlannedMinutes { get; internal set; }
        public double ActualMinutes { get; internal set; }
        public string? Status { get; internal set; }
    }
}
