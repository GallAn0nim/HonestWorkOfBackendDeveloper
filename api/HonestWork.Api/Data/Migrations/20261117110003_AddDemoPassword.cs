using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261117110003_AddDemoPassword : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "DemoPassword",
            table: "Users",
            type: "text",
            nullable: true);

        migrationBuilder.UpdateData(
            table: "Users",
            keyColumn: "Id",
            keyValue: new Guid("27e1fafe-8f69-4e03-a2bc-0f44fd895ac8"),
            column: "DemoPassword",
            value: "admin");

        migrationBuilder.UpdateData(
            table: "Users",
            keyColumn: "Id",
            keyValue: new Guid("6123828f-3f83-45ec-8ccb-0ab7a91de005"),
            column: "DemoPassword",
            value: "test");

        migrationBuilder.UpdateData(
            table: "Users",
            keyColumn: "Id",
            keyValue: new Guid("3f07ae6d-8520-4e9a-a09d-f8ad3f0b7af0"),
            column: "DemoPassword",
            value: "123");

        migrationBuilder.UpdateData(
            table: "Users",
            keyColumn: "Id",
            keyValue: new Guid("57030f65-acd2-452c-b000-4d7d658d5301"),
            column: "DemoPassword",
            value: "admin123");

        migrationBuilder.UpdateData(
            table: "Users",
            keyColumn: "Id",
            keyValue: new Guid("900c4296-f010-4f8c-9643-40393f24d933"),
            column: "DemoPassword",
            value: "honest");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DemoPassword",
            table: "Users");
    }
}
