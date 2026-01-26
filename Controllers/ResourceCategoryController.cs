using Facility_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Facility_Management.Controllers
{
    [ApiController]
    [Route("api/resourcecategories")]
    public class ResourceCategoryController : ControllerBase
    {
        private readonly AppDbContext _context1;

        public ResourceCategoryController(AppDbContext context1)
        {
            _context1 = context1;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        
        public async Task<IActionResult> Create(ResourceCategory category)
        {
            _context1.ResourceCategories.Add(category);
            await _context1.SaveChangesAsync();
            return Ok(category);
        }

        [AllowAnonymous]
        [HttpGet]
        
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context1.ResourceCategories.ToListAsync());
        }
    }
}
