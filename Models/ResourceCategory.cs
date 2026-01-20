namespace Facility_Management.Models
{
    public class ResourceCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
