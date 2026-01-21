namespace Facility_Management.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int ResourceId { get; set; }

        public string ResourceName { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Purpose { get; set; }
        public int NumberOfUsers { get; set; }
        public string? Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
