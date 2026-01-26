using Facility_Management.Models;
using Microsoft.EntityFrameworkCore;


namespace Facility_Management.Repository
{
    public interface IResourceRepository
    {
        Task<List<Resource>> GetAllAsync();
        Task<Resource?> GetByIdAsync(int id);
        Task AddAsync(Resource resource);
        Task UpdateAsync(Resource resource);
        Task ArchiveAsync(int id);

        Task SetMaintenanceStatusAsync(int resourceId, bool isUnderMaintenance); //
    }

    public class ResourceRepository : IResourceRepository
    {
        private readonly AppDbContext _context;

        public ResourceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Resource>> GetAllAsync()
        {
            return await _context.Resource
                .Where(r => !r.IsArchived)
                .Include(r => r.ResourceType)
                .Include(r => r.Category)
                .ToListAsync();
        }

        public async Task<Resource?> GetByIdAsync(int id)
        {
            return await _context.Resource
                .Include(r => r.ResourceType)
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.ResourceId == id);
        }

        public async Task AddAsync(Resource resource)
        {
            _context.Resource.Add(resource);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Resource resource)
        {
            _context.Resource.Update(resource);
            await _context.SaveChangesAsync();
        }

        public async Task ArchiveAsync(int id)
        {
            var resource = await _context.Resource.FindAsync(id);
            if (resource == null) return;

            resource.IsArchived = true;
            await _context.SaveChangesAsync();
        }
        public async Task SetMaintenanceStatusAsync(int resourceId, bool isUnderMaintenance)
        {
            var resource = await _context.Resource.FindAsync(resourceId);
            if (resource == null) return;

            resource.IsUnderMaintenance = isUnderMaintenance;
            resource.IsAvailable = !isUnderMaintenance;
            _context.Resource.Update(resource);
            await _context.SaveChangesAsync();
        }

    }
    }


