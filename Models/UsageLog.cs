using System;

namespace Facility_Management.Models
{
    public class UsageLog
    {
        public int UsageLogId { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; } = default!;

        public DateTime? ActualStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public int? ActualCapacityUsed { get; set; }

        public string Status { get; set; } = "CheckedIn";

        public string Source { get; set; } = "User";
        public int ResourceId { get; internal set; }
        public DateTime StartTime { get; internal set; }
        public DateTime EndTime { get; internal set; }
    }
}


    