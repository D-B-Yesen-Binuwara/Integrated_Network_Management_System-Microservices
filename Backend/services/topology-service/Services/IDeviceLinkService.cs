using topology_service.DTOs;

namespace topology_service.Services;

public interface IDeviceLinkService
{
    Task<DeviceLinkDto> CreateLinkAsync(CreateDeviceLinkDto dto);
    Task<List<DeviceLinkDto>> GetAllLinksAsync();
    Task<bool> DeleteLinkAsync(int id);
}
