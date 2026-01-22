namespace Facility_Management.Models
{
    public class AdminCheckOutDto
    {
        public int BookingId { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public int? ActualCapacityUsed { get; set; }
        public string? Reason { get; set; }
    }
}
