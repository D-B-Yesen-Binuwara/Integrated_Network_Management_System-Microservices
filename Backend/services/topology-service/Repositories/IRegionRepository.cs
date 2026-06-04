using topology_service.Entities;

namespace topology_service.Repositories;

public interface IRegionRepository
{
    Task<List<Region>> GetAllAsync();
    Task<Region?> GetByIdAsync(int id);
    Task<Region> AddAsync(Region region);
    Task<Region> UpdateAsync(Region region);
    Task DeleteAsync(int id);
}
