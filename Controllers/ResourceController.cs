using Facility_Management.DTOs;
using Facility_Management.Models;
using Facility_Management.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Facility_Management.Controllers
{
    [ApiController]
    [Route("api/resources")]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceRepository _repo;

        public ResourceController(IResourceRepository repo)
        {
            _repo = repo;
        }

        [AllowAnonymous]
        [HttpGet]
       
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repo.GetAllAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        
        public async Task<IActionResult> Get(int id)
        {
            var resource = await _repo.GetByIdAsync(id);
            if (resource == null) return NotFound();
            return Ok(resource);
        }

        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpPost]
       
        public async Task<IActionResult> Create(CreateResourceDto dto)
        {
            if (dto.Capacity <= 0)
                return BadRequest("Capacity must be greater than zero");

            var resource = new Resource
            {
                ResourceName = dto.ResourceName,
                ResourceTypeId = dto.ResourceTypeId,
                CategoryId = dto.CategoryId,
                Capacity = dto.Capacity,
                Location = dto.Location
            };

            await _repo.AddAsync(resource);
            return Ok(resource);
        }

        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpPut("{id}")]
        
        public async Task<IActionResult> Update(int id, Resource resource)
        {
            if (id != resource.ResourceId)
                return BadRequest();

            await _repo.UpdateAsync(resource);
            return Ok();
        }

        [Authorize(Policy = "FacilityManagerOrAdmin")]
        [HttpDelete("{id}")]
        
        public async Task<IActionResult> Archive(int id)
        {
            await _repo.ArchiveAsync(id);
            return Ok();
        }
    }
}
