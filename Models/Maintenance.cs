<<<<<<< Updated upstream
﻿namespace Facility_Management.Models
=======
﻿using System.ComponentModel.DataAnnotations;
namespace Facility_Management.Models
>>>>>>> Stashed changes
{
    public class Maintenance
    {
        public int MaintenanceId { get; set; }
        public int ResourceId { get; set; }
<<<<<<< Updated upstream
        public Resource? Resource { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCompleted { get; set; } = false;
=======
        public string? MaintenanceType { get; set; } // Routine / Corrective
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; } // Scheduled / Completed
>>>>>>> Stashed changes
    }
}