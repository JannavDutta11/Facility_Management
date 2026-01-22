namespace Facility_Management.Models
﻿namespace Facility_Management.DTOs
{
    public class CreateResourceDto
    {
        public string ResourceName { get; set; }
        public int ResourceTypeId { get; set; }
        public int CategoryId { get; set; }
        public int Capacity { get; set; }
        public string Location { get; set; }
    }
}
