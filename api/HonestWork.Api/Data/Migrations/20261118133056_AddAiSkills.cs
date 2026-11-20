using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261118133056_AddAiSkills : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "CvSkills",
            columns: new[] { "Id", "ProfileId", "Category", "Name", "SortOrder" },
            values: new object[,]
            {
                { new Guid("f8c911a0-6bf4-4ff4-8244-b0af30ca1056"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "LLM Integration", 25 },
                { new Guid("14f3a02e-6bd6-470c-9664-8b607bacbc95"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "AI Agents", 26 },
                { new Guid("e649fe23-5498-48c6-948d-bcf58c89f61e"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "Prompt Engineering", 27 },
                { new Guid("68ae8098-d39e-4bfc-9bc0-a27ae9769451"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "Ollama", 28 },
                { new Guid("c6ad6ce4-b4be-4b26-8f33-3d9d7040e6d3"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "Claude Code", 29 },
                { new Guid("763a80a4-64b2-48a2-8e79-6313357b7dfe"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "Local LLM Deployment", 30 },
                { new Guid("69110b00-2c97-4e35-9682-fb991a3d0353"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Integracja LLM", 25 },
                { new Guid("525cb84a-13f9-4548-b5b4-9e2f1285f90f"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Agenci AI", 26 },
                { new Guid("9030ed7d-d7d9-422d-8125-016f3d969452"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Prompt engineering", 27 },
                { new Guid("0187b03b-caac-44e2-971a-acefee967c24"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Ollama", 28 },
                { new Guid("9f93147e-6ece-4d28-9ab2-fa9d87f823c9"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Claude Code", 29 },
                { new Guid("e897323d-e012-4f7b-a0f4-86add24faf4e"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Lokalne wdrażanie LLM", 30 }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("f8c911a0-6bf4-4ff4-8244-b0af30ca1056"),
                new Guid("14f3a02e-6bd6-470c-9664-8b607bacbc95"),
                new Guid("e649fe23-5498-48c6-948d-bcf58c89f61e"),
                new Guid("68ae8098-d39e-4bfc-9bc0-a27ae9769451"),
                new Guid("c6ad6ce4-b4be-4b26-8f33-3d9d7040e6d3"),
                new Guid("763a80a4-64b2-48a2-8e79-6313357b7dfe"),
                new Guid("69110b00-2c97-4e35-9682-fb991a3d0353"),
                new Guid("525cb84a-13f9-4548-b5b4-9e2f1285f90f"),
                new Guid("9030ed7d-d7d9-422d-8125-016f3d969452"),
                new Guid("0187b03b-caac-44e2-971a-acefee967c24"),
                new Guid("9f93147e-6ece-4d28-9ab2-fa9d87f823c9"),
                new Guid("e897323d-e012-4f7b-a0f4-86add24faf4e")
            });
    }
}
