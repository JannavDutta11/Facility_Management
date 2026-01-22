using System.ComponentModel.DataAnnotations;
namespace Facility_Management.Models
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        // true = available, false = blocked
        public bool IsAvailable { get; set; } = true;
    }
}