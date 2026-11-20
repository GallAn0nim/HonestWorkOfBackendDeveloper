using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261117120215_FixEnglishHighSchool : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("8b414631-504a-40a8-a9c2-a5e1378a1ded"),
            columns: new[] { "Degree", "Field" },
            values: new object[] { "Technical High School in Białogard", "IT technician" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("8b414631-504a-40a8-a9c2-a5e1378a1ded"),
            columns: new[] { "Degree", "Field" },
            values: new object[] { "Technical High School IT technician", null });
    }
}
