using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261117120432_FixEnglishDegree : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("5ea2acf7-7870-4cf8-b4e6-2e5ebad067e2"),
            column: "Degree",
            value: "Bachelor of Engineering");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("5ea2acf7-7870-4cf8-b4e6-2e5ebad067e2"),
            column: "Degree",
            value: "Engineering degree");
    }
}
