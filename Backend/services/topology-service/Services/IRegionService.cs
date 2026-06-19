using topology_service.Entities;

namespace topology_service.Services;

public interface IRegionService
{
    Task<List<Region>> GetAllRegionsAsync();
    Task<Region?> GetRegionByIdAsync(int id);
    Task<Region> CreateRegionAsync(Region region);
    Task<Region> UpdateRegionAsync(int id, Region region);
    Task DeleteRegionAsync(int id);
}
