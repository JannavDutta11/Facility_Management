namespace Facility_Management.DTOs
{

        public class CreateResourceRuleDto
        {
            public int ResourceId { get; set; }
            public DateTime StartTime{ get; set; }
            public DateTime EndTime { get; set; }
            public int MaxBookingHours { get; set; }
            public int BufferMinutes { get; set; }

        public bool AutoApproveBooking { get; set; }

        public bool AdminApprovalRequired { get; set; }
    }
    }

