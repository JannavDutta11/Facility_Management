namespace Facility_Management.DTOs
{
    public class UserWithRolesDto
    {

        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();

    }
}
