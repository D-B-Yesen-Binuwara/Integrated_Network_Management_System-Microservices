using Microsoft.EntityFrameworkCore;
using topology_service.Entities;
using topology_service.Enums;

namespace topology_service.Data;

public class TopologyDbContext : DbContext
{
    public TopologyDbContext(DbContextOptions<TopologyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<DeviceLink> DeviceLinks => Set<DeviceLink>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresEnum<DeviceType>("device_type");
        modelBuilder.HasPostgresEnum<DeviceStatus>("device_status");
        modelBuilder.HasPostgresEnum<PriorityLevel>("priority_level");

        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("devices");

            entity.HasKey(device => device.DeviceId);

            entity.Property(device => device.DeviceId).HasColumnName("device_id");
            entity.Property(device => device.DeviceName).HasColumnName("device_name");
            entity.Property(device => device.DeviceType).HasColumnName("device_type");
            entity.Property(device => device.IP).HasColumnName("ip");
            entity.Property(device => device.Status).HasColumnName("status");
            entity.Property(device => device.PriorityLevel).HasColumnName("priority_level");
            entity.Property(device => device.Latitude).HasColumnName("latitude");
            entity.Property(device => device.Longitude).HasColumnName("longitude");

            entity.Property(d => d.DeviceType)
                .HasConversion<string>()
                .HasColumnName("device_type");

            entity.Property(d => d.Status)
                .HasConversion<string>()
                .HasColumnName("status");

            entity.Property(d => d.PriorityLevel)
                .HasConversion<string>()
                .HasColumnName("priority_level");
        });

        modelBuilder.Entity<DeviceLink>(entity =>
        {
            entity.ToTable("device_links");

            entity.HasKey(dl => dl.LinkId);

            entity.Property(dl => dl.LinkId).HasColumnName("link_id");
            entity.Property(dl => dl.ParentDeviceId).HasColumnName("parent_device_id");
            entity.Property(dl => dl.ChildDeviceId).HasColumnName("child_device_id");
            entity.Property(dl => dl.LinkStatus).HasColumnName("link_status");

            entity.HasOne(dl => dl.ParentDevice)
                .WithMany()
                .HasForeignKey(dl => dl.ParentDeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(dl => dl.ChildDevice)
                .WithMany()
                .HasForeignKey(dl => dl.ChildDeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
