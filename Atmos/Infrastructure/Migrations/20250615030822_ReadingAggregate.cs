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
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Temperature = table.Column<double>(type: "REAL", nullable: false),
                    TemperatureMinTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    TemperatureMin = table.Column<double>(type: "REAL", nullable: false),
                    TemperatureMaxTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    TemperatureMax = table.Column<double>(type: "REAL", nullable: false),
                    TemperatureOneMinuteAverage = table.Column<double>(type: "REAL", nullable: true),
                    TemperatureFiveMinuteRollingAverage = table.Column<double>(type: "REAL", nullable: true),
                    Humidity = table.Column<double>(type: "REAL", nullable: false),
                    HumidityMinTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    HumidityMin = table.Column<double>(type: "REAL", nullable: false),
                    HumidityMaxTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    HumidityMax = table.Column<double>(type: "REAL", nullable: false),
                    HumidityOneMinuteAverage = table.Column<double>(type: "REAL", nullable: true),
                    HumidityFiveMinuteRollingAverage = table.Column<double>(type: "REAL", nullable: true),
                    DewPoint = table.Column<double>(type: "REAL", nullable: false),
                    DewPointMinTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    DewPointMin = table.Column<double>(type: "REAL", nullable: false),
                    DewPointMaxTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    DewPointMax = table.Column<double>(type: "REAL", nullable: false),
                    DewPointOneMinuteAverage = table.Column<double>(type: "REAL", nullable: true),
                    DewPointFiveMinuteRollingAverage = table.Column<double>(type: "REAL", nullable: true)
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
