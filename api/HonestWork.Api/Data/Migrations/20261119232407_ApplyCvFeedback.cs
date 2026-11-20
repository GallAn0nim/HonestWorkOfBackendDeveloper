using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261119232407_ApplyCvFeedback : Migration
{
    private static readonly Guid ProfileId = new("7b675253-494a-47a9-b7c0-5feea17f6fec");
    private static readonly Guid TeamLeadId = new("82bd2baa-c2a3-4814-b63b-46e21f2f56c7");
    private static readonly Guid ScrumMasterId = new("f50a9b41-da3b-4fb3-ae9c-d08162afbd0b");
    private static readonly Guid DeveloperId = new("eb43e6c0-7693-4262-8c25-069a9d09e508");
    private static readonly Guid EnglishId = new("c2faec80-854e-40f3-8893-8ef033b9b266");
    private static readonly Guid AwsSkillId = new("d7b38fb3-d429-40ee-8c6b-3a91f164cbb2");
    private static readonly Guid LoadTestingSkillId = new("b6d0447e-c714-411b-8358-968787af98dd");
    private static readonly Guid DockerSkillId = new("3f0b8e52-6c1d-4a57-9e2b-7d4c1a9f5e08");
    private static readonly Guid IntegrationTestingSkillId = new("8c2e4f71-0b9a-4d36-a5e1-2f7b6c3d9a14");
    private static readonly Guid ContractTestingSkillId = new("d94a1b3c-7e25-4f80-b6d2-5a8c0e1f3b67");

    private const string CompanySummary = "Software company building EV and automotive software since 2005: automotive PIM, autoparts e-commerce and ERP integrations, used in 12+ countries.";

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CompanySummary",
            table: "CvExperiences",
            type: "character varying(300)",
            maxLength: 300,
            nullable: true);

        SetLanguageLevelLength(migrationBuilder, 100, 50);

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: TeamLeadId,
            columns: new[] { "CompanySummary", "Highlights", "Technologies" },
            values: new object[]
            {
                CompanySummary,
                new[]
                {
                    "Architected unified product data and vehicle data platforms integrating 15 data sources from 8 countries, covering ~30 million products.",
                    "Scaled the team from 11 to 21 members (15 internal, 6 external specialists), leading their onboarding and mentoring.",
                    "Serve as the primary technical contact for client stakeholders, turning business requirements into delivery plans.",
                    "Led the full rollout of an order management service across Germany, handling ~20 million orders annually.",
                    "Manage 24/7 production services (up to 500 req/s) with a 99.98% SLA and zero-downtime deployments."
                },
                new[] { "ASP.NET Core", "Entity Framework Core", "gRPC", "AWS (SQS, DynamoDB, Glue, AppConfig)", "PostgreSQL", "Valkey", "Apigee", "Datadog" }
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: ScrumMasterId,
            columns: new[] { "Highlights", "Technologies" },
            values: new object[]
            {
                new[]
                {
                    "Engineered a rule-based product assortment management system covering 3 million SKUs in 4 countries for B2B and B2C automotive platforms, replacing manual curation.",
                    "Built a unified order management and integration platform, delivering integrations with 15 major ERP systems across Europe.",
                    "Integrated one of the largest automotive ERP systems in Central and Eastern Europe (CEE) over its legacy JSON-RPC API, moving its full order traffic to our platform in a single big-bang cutover with zero lost orders.",
                    "Recruited and onboarded engineers as the team grew from 3 to 11 members, while acting as Scrum Master."
                },
                new[] { ".NET", "ASP.NET Core", "ASP.NET MVC (Razor)", "Entity Framework Core", "REST APIs", "gRPC", "AWS", "Redis", "Apigee" }
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: DeveloperId,
            columns: new[] { "Highlights", "Technologies" },
            values: new object[]
            {
                new[]
                {
                    "Automated pricing with a rule-based system for an international automotive e-commerce platform, moving price updates from once a day to on demand.",
                    "Single-handedly delivered a Sitefinity CMS solution for the customer returns process in the Benelux automotive market, owning client communication and two years of support.",
                    "Built an email template generation system covering 16 email types, so onboarding a new sales channel took only a few CSS changes."
                },
                new[] { "C#", ".NET Framework", "ASP.NET MVC (Razor)", "Entity Framework", "Sitefinity CMS", "SQL Server", "AngularJS", "Node.js", "TypeScript" }
            });

        migrationBuilder.UpdateData(
            table: "CvLanguages",
            keyColumn: "Id",
            keyValue: EnglishId,
            column: "Level",
            value: "B2, daily professional use with international clients");

        migrationBuilder.UpdateData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValue: AwsSkillId,
            column: "Name",
            value: "AWS (ECS, SQS, Glue, AppConfig)");

        migrationBuilder.UpdateData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValue: LoadTestingSkillId,
            column: "SortOrder",
            value: 33);

        migrationBuilder.InsertData(
            table: "CvSkills",
            columns: new[] { "Id", "ProfileId", "Category", "Name", "SortOrder" },
            values: new object[,]
            {
                { IntegrationTestingSkillId, ProfileId, "Testing", "Integration Testing", 12 },
                { DockerSkillId, ProfileId, "Cloud & DevOps", "Docker", 15 },
                { ContractTestingSkillId, ProfileId, "Testing", "Contract Testing", 32 }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[] { IntegrationTestingSkillId, DockerSkillId, ContractTestingSkillId });

        migrationBuilder.UpdateData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValue: LoadTestingSkillId,
            column: "SortOrder",
            value: 13);

        migrationBuilder.UpdateData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValue: AwsSkillId,
            column: "Name",
            value: "AWS (SQS, Glue, AppConfig)");

        migrationBuilder.UpdateData(
            table: "CvLanguages",
            keyColumn: "Id",
            keyValue: EnglishId,
            column: "Level",
            value: "Upper-intermediate (B2)");

        SetLanguageLevelLength(migrationBuilder, 50, 100);

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: TeamLeadId,
            columns: new[] { "Highlights", "Technologies" },
            values: new object[]
            {
                new[]
                {
                    "Architected unified product data and vehicle data platforms integrating multiple data sources across Europe.",
                    "Scaled the team from 11 to 21 members (15 internal, 6 external specialists), leading their onboarding and mentoring.",
                    "Serve as the primary technical contact for client stakeholders, turning business requirements into delivery plans.",
                    "Led the full rollout of an order management service across Germany, handling ~20 million orders annually.",
                    "Manage high-traffic 24/7 production services (up to 500 req/s) with high availability and zero-downtime deployments."
                },
                new[] { "ASP.NET Core", "gRPC", "AWS (SQS, DynamoDB, Glue, AppConfig)", "PostgreSQL", "Valkey", "Apigee", "Datadog" }
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: ScrumMasterId,
            columns: new[] { "Highlights", "Technologies" },
            values: new object[]
            {
                new[]
                {
                    "Engineered a rule-based product assortment management system for international B2B and B2C automotive platforms.",
                    "Built a unified order management and integration platform, delivering integrations with 15 major ERP systems across Europe.",
                    "Integrated one of the largest automotive ERP systems in Central and Eastern Europe (CEE) over its legacy JSON-RPC API, moving its full order traffic to our platform in a single big-bang cutover.",
                    "Recruited and onboarded engineers as the team grew from 3 to 11 members, while acting as Scrum Master."
                },
                new[] { ".NET", "ASP.NET Core", "ASP.NET MVC (Razor)", "REST APIs", "gRPC", "AWS", "Redis", "Apigee" }
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: DeveloperId,
            columns: new[] { "Highlights", "Technologies" },
            values: new object[]
            {
                new[]
                {
                    "Automated pricing with a rule-based system for an international automotive e-commerce platform.",
                    "Single-handedly delivered a Sitefinity CMS solution for the customer returns process in the Benelux automotive market, owning client communication and two years of support.",
                    "Built an email template generation system covering 16 email types, so onboarding a new sales channel took only a few CSS changes."
                },
                new[] { "C#", ".NET Framework", "ASP.NET MVC (Razor)", "Sitefinity CMS", "SQL Server", "AngularJS", "Node.js", "TypeScript" }
            });

        migrationBuilder.DropColumn(
            name: "CompanySummary",
            table: "CvExperiences");
    }

    private static void SetLanguageLevelLength(MigrationBuilder migrationBuilder, int length, int oldLength)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Level",
            table: "CvLanguages",
            type: $"character varying({length})",
            maxLength: length,
            nullable: false,
            oldClrType: typeof(string),
            oldType: $"character varying({oldLength})",
            oldMaxLength: oldLength);
    }
}
