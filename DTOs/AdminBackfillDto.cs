namespace Facility_Management.Models
{
    public class AdminBackfillDto
    {

        public int BookingId { get; set; }
        public DateTime ActualStartTime { get; set; }
        public DateTime ActualEndTime { get; set; }
        public int? ActualCapacityUsed { get; set; }
        public bool EnforceWithinBookingWindow { get; set; } = true;
        public string? Reason { get; set; }

    }
}
