using System.ComponentModel.DataAnnotations.Schema;

namespace Facility_Management.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int ResourceId { get; set; }
        public Resource Resource { get;  set; }
        public string ResourceName { get; set; }
        public int UserId { get; set; }


        [Column("date")]
        public DateTime Date {  get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Purpose { get; set; }
        public int NumberOfUsers { get; set; }
        public string Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }

        [Column("planned_minutes")]
        public int PlannedMinutes { get;  set; }

        [Column("actual_minutes")]
        public int ActualMinutes { get; set; }
    }
}
