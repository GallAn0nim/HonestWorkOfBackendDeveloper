using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261119140000_AddCvTechnologies : Migration
{
    private static readonly string[] TeamLeadStack = [".NET", "ASP.NET Core", "REST", "gRPC", "AWS (SQS, DynamoDB, Glue, AppConfig)", "PostgreSQL", "Valkey", "Apigee", "Datadog"];
    private static readonly string[] ScrumMasterStack = [".NET", "ASP.NET Core", "ASP.NET MVC (Razor)", "REST", "gRPC", "AWS", "Redis", "Apigee"];
    private static readonly string[] JuniorStack = ["C#", ".NET Framework", "ASP.NET MVC (Razor)", "Sitefinity CMS", "SQL Server", "AngularJS", "Node.js", "TypeScript"];

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string[]>(
            name: "Technologies",
            table: "CvExperiences",
            type: "text[]",
            nullable: false,
            defaultValue: new string[0]);

        SetTechnologies(migrationBuilder, "82bd2baa-c2a3-4814-b63b-46e21f2f56c7", TeamLeadStack);
        SetTechnologies(migrationBuilder, "470017b4-2bea-4dcc-a8e2-7f363264b5f4", TeamLeadStack);
        SetTechnologies(migrationBuilder, "f50a9b41-da3b-4fb3-ae9c-d08162afbd0b", ScrumMasterStack);
        SetTechnologies(migrationBuilder, "433d41dc-6a86-4d25-99ff-3a502e0a3600", ScrumMasterStack);
        SetTechnologies(migrationBuilder, "eb43e6c0-7693-4262-8c25-069a9d09e508", JuniorStack);
        SetTechnologies(migrationBuilder, "2214aa78-3923-4498-8871-336ed00bad36", JuniorStack);

        migrationBuilder.InsertData(
            table: "CvSkills",
            columns: new[] { "Id", "ProfileId", "Category", "Name", "SortOrder" },
            values: new object[,]
            {
                { new Guid("2c81be17-e388-4252-8d13-77f8f6588e29"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Databases & Caching", "DynamoDB", 31 },
                { new Guid("5d9e459b-660d-4b76-938f-0703ecfe3791"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "AWS Glue", 32 },
                { new Guid("080b7b79-10c0-48aa-be0c-2e92d20948f7"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "AWS AppConfig", 33 },
                { new Guid("e6ec1531-0198-4f12-b528-be291cb70ac5"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Bazy danych i cache", "DynamoDB", 31 },
                { new Guid("72b3cffe-7af9-4a77-a609-1e032c614b75"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "AWS Glue", 32 },
                { new Guid("22cca6a4-09aa-489e-b52d-7706d6c07a64"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "AWS AppConfig", 33 }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("2c81be17-e388-4252-8d13-77f8f6588e29"),
                new Guid("5d9e459b-660d-4b76-938f-0703ecfe3791"),
                new Guid("080b7b79-10c0-48aa-be0c-2e92d20948f7"),
                new Guid("e6ec1531-0198-4f12-b528-be291cb70ac5"),
                new Guid("72b3cffe-7af9-4a77-a609-1e032c614b75"),
                new Guid("22cca6a4-09aa-489e-b52d-7706d6c07a64")
            });

        migrationBuilder.DropColumn(
            name: "Technologies",
            table: "CvExperiences");
    }

    private static void SetTechnologies(MigrationBuilder migrationBuilder, string experienceId, string[] technologies)
    {
        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid(experienceId),
            column: "Technologies",
            value: technologies);
    }
}
