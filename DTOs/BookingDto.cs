namespace Facility_Management.DTOs
{
    public class BookingDto
    {
        public int ResourceId { get; set; }

        //public string ResourceName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string? Purpose { get; set; }

        public int NumberOfUsers { get; set; }

    }
}
    

