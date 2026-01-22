namespace Facility_Management.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }

        public int ResourceTypeId { get; set; }
        public ResourceType ResourceType { get; set; }

        public int CategoryId { get; set; }
        public ResourceCategory Category { get; set; }

        public int Capacity { get; set; }
        public string Location { get; set; }

        public bool IsArchived { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsAvailable { get; set; }
        public bool IsUnderMaintenance { get;  set; }
    }
}
