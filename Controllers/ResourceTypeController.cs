using Facility_Management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Facility_Management.Controllers
{
    [ApiController]
    [Route("api/resourcetypes")]
    public class ResourceTypeController : ControllerBase
    {
        private readonly AppDbContext _context1;

        public ResourceTypeController(AppDbContext context1)
        {
            _context1 = context1;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResourceType type)
        {
            _context1.ResourceTypes.Add(type);
            await _context1.SaveChangesAsync();
            return Ok(type);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context1.ResourceTypes.ToListAsync());
        }
    }
}
