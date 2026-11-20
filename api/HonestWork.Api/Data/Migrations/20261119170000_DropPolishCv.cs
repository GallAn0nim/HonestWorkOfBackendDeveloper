using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261119170000_DropPolishCv : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "CvProfiles",
            keyColumn: "Id",
            keyValue: new Guid("81735658-6291-4cb7-accc-32011498c07d"));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        throw new NotSupportedException("The Polish CV was removed for good; restore it from the migrations before 20261119170000_DropPolishCv.");
    }
}
