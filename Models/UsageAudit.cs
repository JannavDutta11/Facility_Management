namespace Facility_Management.Models
{
    public class UsageAudit
    {
        public int UsageAuditId { get; set; }
        public int BookingId { get; set; }
        public int? UsageLogId { get; set; }

        // "AdminCheckIn" | "AdminCheckOut" | "AdminBackfill" | "AdminEdit" | "MarkNoShow" | "UndoNoShow" | "AutoNoShow"
        public string Action { get; set; } = default!;

        public string ChangedBy { get; set; } = "system";
        public DateTime ChangedAt { get; set; } = DateTime.Now;

        public string? Reason { get; set; }
        public string? DiffJson { get; set; }
    }
}

       
    
