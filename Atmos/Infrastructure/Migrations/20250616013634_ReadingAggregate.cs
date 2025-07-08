using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReadingAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReadingAggregates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    Temperature = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMinTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TemperatureMin = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureMaxTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TemperatureMax = table.Column<double>(type: "double precision", nullable: false),
                    TemperatureOneMinuteAverage = table.Column<double>(type: "double precision", nullable: true),
                    TemperatureFiveMinuteRollingAverage = table.Column<double>(type: "double precision", nullable: true),
                    Humidity = table.Column<double>(type: "double precision", nullable: false),
                    HumidityMinTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HumidityMin = table.Column<double>(type: "double precision", nullable: false),
                    HumidityMaxTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HumidityMax = table.Column<double>(type: "double precision", nullable: false),
                    HumidityOneMinuteAverage = table.Column<double>(type: "double precision", nullable: true),
                    HumidityFiveMinuteRollingAverage = table.Column<double>(type: "double precision", nullable: true),
                    DewPoint = table.Column<double>(type: "double precision", nullable: false),
                    DewPointMinTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DewPointMin = table.Column<double>(type: "double precision", nullable: false),
                    DewPointMaxTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DewPointMax = table.Column<double>(type: "double precision", nullable: false),
                    DewPointOneMinuteAverage = table.Column<double>(type: "double precision", nullable: true),
                    DewPointFiveMinuteRollingAverage = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingAggregates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReadingAggregates");
        }
    }
}
