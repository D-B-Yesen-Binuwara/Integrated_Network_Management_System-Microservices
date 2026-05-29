using topology_service.Entities;

namespace topology_service.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly List<Device> _devices = new();
    private int _nextId = 1;

    public IEnumerable<Device> GetAll()
    {
        return _devices.OrderBy(device => device.DeviceId).ToList();
    }

    public Device? GetById(int id)
    {
        return _devices.FirstOrDefault(device => device.DeviceId == id);
    }

    public Device Create(Device device)
    {
        device.DeviceId = _nextId++;
        _devices.Add(device);
        return device;
    }

    public Device? Update(int id, Device device)
    {
        var existing = GetById(id);
        if (existing == null)
        {
            return null;
        }

        existing.DeviceName = device.DeviceName;
        existing.DeviceType = device.DeviceType;
        existing.IP = device.IP;
        existing.Status = device.Status;
        existing.PriorityLevel = device.PriorityLevel;
        existing.Latitude = device.Latitude;
        existing.Longitude = device.Longitude;

        return existing;
    }

    public bool Delete(int id)
    {
        var existing = GetById(id);
        if (existing == null)
        {
            return false;
        }

        _devices.Remove(existing);
        return true;
    }
}
