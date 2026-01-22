namespace Facility_Management.Models
{
    public class AdminCheckInDto
    {
        public int BookingId { get; set; }
        public DateTime? ActualStartTime { get; set; }
        public string? Reason { get; set; }
    }
}
