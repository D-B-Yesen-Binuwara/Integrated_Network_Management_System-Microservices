using Microsoft.EntityFrameworkCore;
using topology_service.Data;
using topology_service.Entities;

namespace topology_service.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly TopologyDbContext _context;

    public DeviceRepository(TopologyDbContext context)
    {
        _context = context;
    }

    public async Task<List<Device>> GetAllAsync()
    {
        return await _context.Devices.AsNoTracking().OrderBy(device => device.DeviceId).ToListAsync();
    }

    public async Task<Device?> GetByIdAsync(int id)
    {
        return await _context.Devices.AsNoTracking().FirstOrDefaultAsync(device => device.DeviceId == id);
    }

    public async Task AddAsync(Device device)
    {
        await _context.Devices.AddAsync(device);
        await _context.SaveChangesAsync();
    }

    public async Task<Device?> UpdateAsync(int id, Device device)
    {
        var existing = await _context.Devices.FirstOrDefaultAsync(current => current.DeviceId == id);
        if (existing == null)
        {
            return null;
        }

        existing.DeviceName = device.DeviceName;
        existing.DeviceType = device.DeviceType;
        existing.IP = device.IP;
        existing.RegionCode = device.RegionCode;
        existing.ProvinceCode = device.ProvinceCode;
        existing.LEACode = device.LEACode;
        existing.AssignedEngineerId = device.AssignedEngineerId;
        existing.Status = device.Status;
        existing.PriorityLevel = device.PriorityLevel;
        existing.Latitude = device.Latitude;
        existing.Longitude = device.Longitude;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Devices.FirstOrDefaultAsync(device => device.DeviceId == id);
        if (existing == null)
        {
            return false;
        }

        _context.Devices.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
