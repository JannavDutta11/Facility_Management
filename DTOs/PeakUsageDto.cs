namespace Facility_Management.DTOs
{
    public class PeakUsageDto
    {
        public string ResourceName { get; set; }
        public int PeakHour { get; set; }
        public int BookingCount { get; set; }
    }
}
