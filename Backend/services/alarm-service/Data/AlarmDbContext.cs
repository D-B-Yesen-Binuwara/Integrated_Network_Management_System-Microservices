using Microsoft.EntityFrameworkCore;
using alarm_service.Entities;

namespace alarm_service.Data;

public class AlarmDbContext : DbContext
{
    public AlarmDbContext(DbContextOptions<AlarmDbContext> options) : base(options)
    {
    }

    public DbSet<MSANAlarm> MSANAlarms => Set<MSANAlarm>();
    public DbSet<SLBNAlarm> SLBNAlarms => Set<SLBNAlarm>();
    public DbSet<CEAAlarm> CEAAlarms => Set<CEAAlarm>();
    public DbSet<RootCause> RootCauses => Set<RootCause>();
    public DbSet<ImpactedDevice> ImpactedDevices => Set<ImpactedDevice>();
    public DbSet<CorrelatedFault> CorrelatedFaults => Set<CorrelatedFault>();
    public DbSet<CorrelatedFaultAlarm> CorrelatedFaultAlarms => Set<CorrelatedFaultAlarm>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MSANAlarm>(entity =>
        {
            entity.ToTable("MSANAlarm");
            entity.HasKey(e => e.MSANAlarmId);

            entity.Property(e => e.MSANAlarmId).HasColumnName("MSANAlarmId");
            entity.Property(e => e.DeviceId).HasColumnName("DeviceId");
            entity.Property(e => e.RegionCode).HasColumnName("RegionCode").HasMaxLength(20);
            entity.Property(e => e.ProvinceCode).HasColumnName("ProvinceCode").HasMaxLength(20);
            entity.Property(e => e.LEACode).HasColumnName("LEACode").HasMaxLength(20);
            entity.Property(e => e.AlarmType).HasColumnName("AlarmType");
            entity.Property(e => e.RaisedTime).HasColumnName("RaisedTime");
            entity.Property(e => e.ClearedTime).HasColumnName("ClearedTime");
            entity.Property(e => e.IsActive).HasColumnName("IsActive");
        });

        modelBuilder.Entity<SLBNAlarm>(entity =>
        {
            entity.ToTable("SLBNAlarm");
            entity.HasKey(e => e.SLBNAlarmId);

            entity.Property(e => e.SLBNAlarmId).HasColumnName("SLBNAlarmId");
            entity.Property(e => e.DeviceId).HasColumnName("DeviceId");
            entity.Property(e => e.RegionCode).HasColumnName("RegionCode").HasMaxLength(20);
            entity.Property(e => e.ProvinceCode).HasColumnName("ProvinceCode").HasMaxLength(20);
            entity.Property(e => e.LEACode).HasColumnName("LEACode").HasMaxLength(20);
            entity.Property(e => e.AlarmType).HasColumnName("AlarmType");
            entity.Property(e => e.RaisedTime).HasColumnName("RaisedTime");
            entity.Property(e => e.ClearedTime).HasColumnName("ClearedTime");
            entity.Property(e => e.IsActive).HasColumnName("IsActive");
        });

        modelBuilder.Entity<CEAAlarm>(entity =>
        {
            entity.ToTable("CEAAlarm");
            entity.HasKey(e => e.CEAAlarmId);

            entity.Property(e => e.CEAAlarmId).HasColumnName("CEAAlarmId");
            entity.Property(e => e.DeviceId).HasColumnName("DeviceId");
            entity.Property(e => e.RegionCode).HasColumnName("RegionCode").HasMaxLength(20);
            entity.Property(e => e.ProvinceCode).HasColumnName("ProvinceCode").HasMaxLength(20);
            entity.Property(e => e.LEACode).HasColumnName("LEACode").HasMaxLength(20);
            entity.Property(e => e.AlarmType).HasColumnName("AlarmType");
            entity.Property(e => e.RaisedTime).HasColumnName("RaisedTime");
            entity.Property(e => e.ClearedTime).HasColumnName("ClearedTime");
            entity.Property(e => e.IsActive).HasColumnName("IsActive");
        });

        modelBuilder.Entity<RootCause>(entity =>
        {
            // Keep the table name explicit because the manual SQL uses the DbSet names.
            entity.ToTable("RootCauses");
            entity.HasKey(e => e.RootCauseId);
            entity.HasIndex(e => e.DeviceId);
            entity.HasIndex(e => e.AlarmId);
            entity.Property(e => e.SourceDeviceType).HasMaxLength(50);
            entity.Property(e => e.CorrelationRuleName).HasMaxLength(150);

            entity.HasMany(r => r.ImpactedDevices)
                .WithOne(i => i.RootCause)
                .HasForeignKey(i => i.RootCauseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ImpactedDevice>(entity =>
        {
            // Indexes support root-cause detail pages and device impact lookups.
            entity.ToTable("ImpactedDevices");
            entity.HasKey(e => e.ImpactedDeviceId);
            entity.HasIndex(e => e.RootCauseId);
            entity.HasIndex(e => e.DeviceId);
            entity.Property(e => e.DeviceType).HasMaxLength(50);
        });

        modelBuilder.Entity<CorrelatedFault>(entity =>
        {
            entity.ToTable("CorrelatedFaults");
            entity.HasKey(e => e.CorrelatedFaultId);
            entity.HasIndex(e => e.RootCauseId);
            entity.HasIndex(e => e.SourceDeviceId);
            entity.HasIndex(e => new { e.SourceDeviceType, e.SourceAlarmId });
            entity.HasMany(e => e.SuppressedAlarms)
                .WithOne(e => e.CorrelatedFault)
                .HasForeignKey(e => e.CorrelatedFaultId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.RootCause)
                .WithMany()
                .HasForeignKey(e => e.RootCauseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CorrelatedFaultAlarm>(entity =>
        {
            entity.ToTable("CorrelatedFaultAlarms");
            entity.HasKey(e => e.CorrelatedFaultAlarmId);
            entity.HasIndex(e => new { e.DeviceType, e.AlarmId });
        });
    }
}



