using topology_service.Entities;

namespace topology_service.Repositories;

public interface IDeviceRepository
{
    IEnumerable<Device> GetAll();
    Device? GetById(int id);
    Device Create(Device device);
    Device? Update(int id, Device device);
    bool Delete(int id);
}
