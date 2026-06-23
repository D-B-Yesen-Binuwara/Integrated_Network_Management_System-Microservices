using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace alarm_service.Migrations
{
    /// <inheritdoc />
    public partial class InitialAlarmDomainSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CEAAlarm",
                columns: table => new
                {
                    CEAAlarmId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    AlarmType = table.Column<string>(type: "text", nullable: false),
                    RaisedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClearedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CEAAlarm", x => x.CEAAlarmId);
                });

            migrationBuilder.CreateTable(
                name: "MSANAlarm",
                columns: table => new
                {
                    MSANAlarmId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    AlarmType = table.Column<string>(type: "text", nullable: false),
                    RaisedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClearedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MSANAlarm", x => x.MSANAlarmId);
                });

            migrationBuilder.CreateTable(
                name: "SLBNAlarm",
                columns: table => new
                {
                    SLBNAlarmId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    AlarmType = table.Column<string>(type: "text", nullable: false),
                    RaisedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClearedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SLBNAlarm", x => x.SLBNAlarmId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CEAAlarm");

            migrationBuilder.DropTable(
                name: "MSANAlarm");

            migrationBuilder.DropTable(
                name: "SLBNAlarm");
        }
    }
}
