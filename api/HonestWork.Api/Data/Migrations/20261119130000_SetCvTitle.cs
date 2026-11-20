using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261119130000_SetCvTitle : Migration
{
    private const string NewTitle = "Team Lead · .NET Backend";
    private const string OldTitle = "Team Lead | Senior .NET Developer";

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        SetTitle(migrationBuilder, NewTitle);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        SetTitle(migrationBuilder, OldTitle);
    }

    private static void SetTitle(MigrationBuilder migrationBuilder, string title)
    {
        migrationBuilder.UpdateData(
            table: "CvProfiles",
            keyColumn: "Id",
            keyValues: new object[] { new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), new Guid("81735658-6291-4cb7-accc-32011498c07d") },
            column: "Title",
            values: new object[] { title, title });
    }
}
