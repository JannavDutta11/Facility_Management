namespace Facility_Management.Models
{
    public class ResourceType
    {
        public int ResourceTypeId { get; set; }
        public string TypeName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
