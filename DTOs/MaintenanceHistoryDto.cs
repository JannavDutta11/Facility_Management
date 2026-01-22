namespace Facility_Management.Dtos
{
    public class MaintenanceHistoryDto
    {
        public int ResourceId { get; set; }
        public string WorkDone { get; set; }
        public int Cost { get; set; }
        public int TimeTakenHours { get; set; }
        public string PartsUsed { get; set; }
    }
}