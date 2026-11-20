using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261117115950_FixEnglishEducation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("5ea2acf7-7870-4cf8-b4e6-2e5ebad067e2"),
            columns: new[] { "Degree", "Field", "Thesis" },
            values: new object[] { "Engineering degree", "Computer Programming and Networking", "The use of artificial intelligence algorithms to search and track player’s location based on a simple arcade game" });

        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("8b414631-504a-40a8-a9c2-a5e1378a1ded"),
            columns: new[] { "Degree" },
            values: new object[] { "Technical High School IT technician" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("5ea2acf7-7870-4cf8-b4e6-2e5ebad067e2"),
            columns: new[] { "Degree", "Field", "Thesis" },
            values: new object[] { "Engineer", "Computer Programming and Information Networks", "Application of selected artificial intelligence algorithms to finding and tracking a player in a simple arcade game" });

        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("8b414631-504a-40a8-a9c2-a5e1378a1ded"),
            columns: new[] { "Degree" },
            values: new object[] { "Technical school, computer science" });
    }
}
