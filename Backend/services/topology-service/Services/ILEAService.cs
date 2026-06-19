using topology_service.Entities;

namespace topology_service.Services;

public interface ILEAService
{
    Task<List<LEA>> GetAllAsync();
    Task<LEA?> GetByIdAsync(int id);
    Task<LEA> CreateAsync(LEA lea);
    Task<LEA> UpdateAsync(int id, LEA lea);
    Task DeleteAsync(int id);
}
