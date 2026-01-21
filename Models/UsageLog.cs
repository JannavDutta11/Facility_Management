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

        // Usage record state (NOT Booking.Status)
        // "CheckedIn" | "CheckedOut" | "NoShow" | "ManuallyUpdated"
        public string Status { get; set; } = "CheckedIn";

        // "User" | "System" | "Admin"
        public string Source { get; set; } = "User";
    }
}


    