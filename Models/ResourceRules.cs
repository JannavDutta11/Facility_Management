namespace Facility_Management.Models
{
    public class ResourceRule
    {
        public int RuleId { get; set; }
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }

        public int StartHour { get; set; }
        public int EndHour { get; set; }

        public int MaxBookingHours { get; set; }
        public int BufferMinutes { get; set; }
    }
}
