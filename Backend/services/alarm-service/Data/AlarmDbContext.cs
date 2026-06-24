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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MSANAlarm>(entity =>
        {
            entity.ToTable("MSANAlarm");
            entity.HasKey(e => e.MSANAlarmId);

            entity.Property(e => e.MSANAlarmId).HasColumnName("MSANAlarmId");
            entity.Property(e => e.DeviceId).HasColumnName("DeviceId");
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
            entity.Property(e => e.AlarmType).HasColumnName("AlarmType");
            entity.Property(e => e.RaisedTime).HasColumnName("RaisedTime");
            entity.Property(e => e.ClearedTime).HasColumnName("ClearedTime");
            entity.Property(e => e.IsActive).HasColumnName("IsActive");
        });

    }
}



