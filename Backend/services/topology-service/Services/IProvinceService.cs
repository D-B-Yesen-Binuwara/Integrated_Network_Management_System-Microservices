using topology_service.Entities;

namespace topology_service.Services;

public interface IProvinceService
{
    Task<List<Province>> GetAllAsync();
    Task<Province?> GetByIdAsync(int id);
    Task<Province> CreateAsync(Province province);
    Task<Province> UpdateAsync(int id, Province province);
    Task DeleteAsync(int id);
}
