using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261119160000_TrimCvTechLines : Migration
{
    private static readonly string[] TeamLeadStack = ["ASP.NET Core", "gRPC", "AWS (SQS, DynamoDB, Glue, AppConfig)", "PostgreSQL", "Valkey", "Apigee", "Datadog"];
    private static readonly string[] OldTeamLeadStack = [".NET", "ASP.NET Core", "gRPC", "AWS (SQS, DynamoDB, Glue, AppConfig)", "PostgreSQL", "Valkey", "Apigee", "Datadog"];

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        SetTeamLeadStack(migrationBuilder, TeamLeadStack);

        migrationBuilder.DeleteData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("eb448047-9679-49f8-8b77-711a3ad7d8c4"),
                new Guid("f6c765fd-1ebd-4be4-8925-07a2b056935b")
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "CvSkills",
            columns: new[] { "Id", "ProfileId", "Category", "Name", "SortOrder" },
            values: new object[,]
            {
                { new Guid("eb448047-9679-49f8-8b77-711a3ad7d8c4"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "Docker", 16 },
                { new Guid("f6c765fd-1ebd-4be4-8925-07a2b056935b"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "Docker", 16 }
            });

        SetTeamLeadStack(migrationBuilder, OldTeamLeadStack);
    }

    private static void SetTeamLeadStack(MigrationBuilder migrationBuilder, string[] technologies)
    {
        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValues: new object[] { new Guid("82bd2baa-c2a3-4814-b63b-46e21f2f56c7"), new Guid("470017b4-2bea-4dcc-a8e2-7f363264b5f4") },
            column: "Technologies",
            values: new object[] { technologies, technologies });
    }
}
