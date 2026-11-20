using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261118132633_SetEnglishB2 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "CvLanguages",
            keyColumn: "Id",
            keyValue: new Guid("c2faec80-854e-40f3-8893-8ef033b9b266"),
            column: "Level",
            value: "Upper-intermediate (B2)");

        migrationBuilder.UpdateData(
            table: "CvLanguages",
            keyColumn: "Id",
            keyValue: new Guid("7d08bec8-8ccb-4f91-a24a-56c88b36c991"),
            column: "Level",
            value: "Średniozaawansowany wyższy (B2)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "CvLanguages",
            keyColumn: "Id",
            keyValue: new Guid("c2faec80-854e-40f3-8893-8ef033b9b266"),
            column: "Level",
            value: "Intermediate (B1)");

        migrationBuilder.UpdateData(
            table: "CvLanguages",
            keyColumn: "Id",
            keyValue: new Guid("7d08bec8-8ccb-4f91-a24a-56c88b36c991"),
            column: "Level",
            value: "Średniozaawansowany (B1)");
    }
}
