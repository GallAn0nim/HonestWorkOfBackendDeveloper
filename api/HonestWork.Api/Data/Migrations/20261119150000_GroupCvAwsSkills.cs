using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261119150000_GroupCvAwsSkills : Migration
{
    private static readonly string[] TeamLeadStack = [".NET", "ASP.NET Core", "gRPC", "AWS (SQS, DynamoDB, Glue, AppConfig)", "PostgreSQL", "Valkey", "Apigee", "Datadog"];
    private static readonly string[] ScrumMasterStack = [".NET", "ASP.NET Core", "ASP.NET MVC (Razor)", "REST APIs", "gRPC", "AWS", "Redis", "Apigee"];
    private static readonly string[] OldTeamLeadStack = [".NET", "ASP.NET Core", "REST", "gRPC", "AWS (SQS, DynamoDB, Glue, AppConfig)", "PostgreSQL", "Valkey", "Apigee", "Datadog"];
    private static readonly string[] OldScrumMasterStack = [".NET", "ASP.NET Core", "ASP.NET MVC (Razor)", "REST", "gRPC", "AWS", "Redis", "Apigee"];

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        SetTechnologies(migrationBuilder, "82bd2baa-c2a3-4814-b63b-46e21f2f56c7", TeamLeadStack);
        SetTechnologies(migrationBuilder, "470017b4-2bea-4dcc-a8e2-7f363264b5f4", TeamLeadStack);
        SetTechnologies(migrationBuilder, "f50a9b41-da3b-4fb3-ae9c-d08162afbd0b", ScrumMasterStack);
        SetTechnologies(migrationBuilder, "433d41dc-6a86-4d25-99ff-3a502e0a3600", ScrumMasterStack);

        migrationBuilder.UpdateData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[] { new Guid("d7b38fb3-d429-40ee-8c6b-3a91f164cbb2"), new Guid("6d8ea0ff-f1f6-4aef-b272-0372949e8281") },
            column: "Name",
            values: new object[] { "AWS (SQS, Glue, AppConfig)", "AWS (SQS, Glue, AppConfig)" });

        migrationBuilder.DeleteData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("3b975849-5ca6-46c9-b167-3cdb83e1fd74"),
                new Guid("5d9e459b-660d-4b76-938f-0703ecfe3791"),
                new Guid("080b7b79-10c0-48aa-be0c-2e92d20948f7"),
                new Guid("99e4a56e-4846-4680-b6dc-5e65c5f6427c"),
                new Guid("72b3cffe-7af9-4a77-a609-1e032c614b75"),
                new Guid("22cca6a4-09aa-489e-b52d-7706d6c07a64")
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "CvSkills",
            columns: new[] { "Id", "ProfileId", "Category", "Name", "SortOrder" },
            values: new object[,]
            {
                { new Guid("3b975849-5ca6-46c9-b167-3cdb83e1fd74"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "SQS", 15 },
                { new Guid("5d9e459b-660d-4b76-938f-0703ecfe3791"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "AWS Glue", 32 },
                { new Guid("080b7b79-10c0-48aa-be0c-2e92d20948f7"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "AWS AppConfig", 33 },
                { new Guid("99e4a56e-4846-4680-b6dc-5e65c5f6427c"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "SQS", 15 },
                { new Guid("72b3cffe-7af9-4a77-a609-1e032c614b75"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "AWS Glue", 32 },
                { new Guid("22cca6a4-09aa-489e-b52d-7706d6c07a64"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "AWS AppConfig", 33 }
            });

        migrationBuilder.UpdateData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[] { new Guid("d7b38fb3-d429-40ee-8c6b-3a91f164cbb2"), new Guid("6d8ea0ff-f1f6-4aef-b272-0372949e8281") },
            column: "Name",
            values: new object[] { "AWS", "AWS" });

        SetTechnologies(migrationBuilder, "82bd2baa-c2a3-4814-b63b-46e21f2f56c7", OldTeamLeadStack);
        SetTechnologies(migrationBuilder, "470017b4-2bea-4dcc-a8e2-7f363264b5f4", OldTeamLeadStack);
        SetTechnologies(migrationBuilder, "f50a9b41-da3b-4fb3-ae9c-d08162afbd0b", OldScrumMasterStack);
        SetTechnologies(migrationBuilder, "433d41dc-6a86-4d25-99ff-3a502e0a3600", OldScrumMasterStack);
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
