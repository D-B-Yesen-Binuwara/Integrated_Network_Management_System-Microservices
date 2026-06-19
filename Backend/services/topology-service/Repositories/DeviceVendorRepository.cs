using Microsoft.EntityFrameworkCore;
using topology_service.Entities;
using topology_service.Repositories;
using topology_service.Data;

namespace topology_service.Repositories;

public class DeviceVendorRepository : IDeviceVendorRepository
{
    private readonly TopologyDbContext _context;

    public DeviceVendorRepository(TopologyDbContext context)
    {
        _context = context;
    }

    public async Task<DeviceVendor?> GetByIdAsync(int id)
    {
        return await _context.DeviceVendors
            .Include(dv => dv.Device)
            .Include(dv => dv.Vendor)
            .FirstOrDefaultAsync(dv => dv.DeviceVendorId == id);
    }

    public async Task<List<DeviceVendor>> GetByVendorIdAsync(int vendorId)
    {
        return await _context.DeviceVendors
            .Include(dv => dv.Device)
            .Where(dv => dv.VendorId == vendorId)
            .ToListAsync();
    }

    public async Task<List<DeviceVendor>> GetByDeviceIdAsync(int deviceId)
    {
        return await _context.DeviceVendors
            .Include(dv => dv.Vendor)
            .Where(dv => dv.DeviceId == deviceId)
            .ToListAsync();
    }

    public async Task<int> GetActiveDeviceCountAsync(int vendorId)
    {
        return await _context.DeviceVendors
            .Where(dv => dv.VendorId == vendorId && dv.IsActive)
            .CountAsync();
    }

    public async Task<int> GetTotalDeviceCountAsync(int vendorId)
    {
        return await _context.DeviceVendors
            .Where(dv => dv.VendorId == vendorId)
            .CountAsync();
    }

    public async Task<DateTime?> GetLastAssignmentDateAsync(int vendorId)
    {
        return await _context.DeviceVendors
            .Where(dv => dv.VendorId == vendorId)
            .OrderByDescending(dv => dv.AssignedDate)
            .Select(dv => dv.AssignedDate)
            .FirstOrDefaultAsync();
    }

    public async Task<List<DeviceVendor>> GetRecentAssignmentsAsync(int vendorId, int limit = 5)
    {
        return await _context.DeviceVendors
            .Include(dv => dv.Device)
            .Where(dv => dv.VendorId == vendorId)
            .OrderByDescending(dv => dv.AssignedDate)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(DeviceVendor deviceVendor)
    {
        await _context.DeviceVendors.AddAsync(deviceVendor);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DeviceVendor deviceVendor)
    {
        _context.DeviceVendors.Update(deviceVendor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(DeviceVendor deviceVendor)
    {
        _context.DeviceVendors.Remove(deviceVendor);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int deviceId, int vendorId)
    {
        return await _context.DeviceVendors
            .AnyAsync(dv => dv.DeviceId == deviceId && dv.VendorId == vendorId);
    }
}
