using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261119120000_RefineCvSkillsAndErp : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValue: new Guid("1d445487-509b-4bf3-bd4f-839aa9733fea"),
            column: "Name",
            value: "Ollama");

        migrationBuilder.UpdateData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValue: new Guid("8984bfae-6e59-4ba2-816e-4e65bc877f06"),
            column: "Name",
            value: "Ollama");

        migrationBuilder.DeleteData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("dfffc670-a01a-4270-b534-84394c6d43a9"),
                new Guid("41ef0c07-6131-410e-92bd-3548e6d23f28")
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("f50a9b41-da3b-4fb3-ae9c-d08162afbd0b"),
            column: "Highlights",
            value: new[]
            {
                "Engineered a rule-based product assortment management system for international B2B and B2C automotive platforms.",
                "Built a unified order management and integration platform, delivering integrations with 15 major ERP systems across Europe.",
                "Integrated one of the largest automotive ERP systems in Central and Eastern Europe (CEE) over its legacy JSON-RPC API, moving its full order traffic to our platform in a single big-bang cutover.",
                "Recruited and onboarded engineers as the team grew from 3 to 11 members, while acting as Scrum Master."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("433d41dc-6a86-4d25-99ff-3a502e0a3600"),
            column: "Highlights",
            value: new[]
            {
                "Opracowałem regułowy system zarządzania asortymentem produktów dla międzynarodowych platform motoryzacyjnych B2B i B2C.",
                "Zbudowałem ujednoliconą platformę zarządzania zamówieniami i integracji, dostarczając integracje z 15 dużymi systemami ERP w Europie.",
                "Zintegrowałem jeden z największych motoryzacyjnych systemów ERP w Europie Środkowo-Wschodniej (CEE) przez jego starsze API JSON-RPC, przenosząc cały ruch zamówień na naszą platformę w jednym przełączeniu (big bang).",
                "Rekrutowałem i wdrażałem nowe osoby, gdy zespół rósł z 3 do 11 osób, pełniąc jednocześnie rolę Scrum Mastera."
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("f50a9b41-da3b-4fb3-ae9c-d08162afbd0b"),
            column: "Highlights",
            value: new[]
            {
                "Engineered a rule-based product assortment management system for international B2B and B2C automotive platforms.",
                "Built a unified order management and integration platform, delivering integrations with 15 major ERP systems across Europe.",
                "Integrated one of the largest automotive ERP systems in Central and Eastern Europe (CEE).",
                "Recruited and onboarded engineers as the team grew from 3 to 11 members, while acting as Scrum Master."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("433d41dc-6a86-4d25-99ff-3a502e0a3600"),
            column: "Highlights",
            value: new[]
            {
                "Opracowałem regułowy system zarządzania asortymentem produktów dla międzynarodowych platform motoryzacyjnych B2B i B2C.",
                "Zbudowałem ujednoliconą platformę zarządzania zamówieniami i integracji, dostarczając integracje z 15 dużymi systemami ERP w Europie.",
                "Zintegrowałem jeden z największych motoryzacyjnych systemów ERP w Europie Środkowo-Wschodniej (CEE).",
                "Rekrutowałem i wdrażałem nowe osoby, gdy zespół rósł z 3 do 11 osób, pełniąc jednocześnie rolę Scrum Mastera."
            });

        migrationBuilder.InsertData(
            table: "CvSkills",
            columns: new[] { "Id", "ProfileId", "Category", "Name", "SortOrder" },
            values: new object[,]
            {
                { new Guid("dfffc670-a01a-4270-b534-84394c6d43a9"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Testing", "Integration Tests", 12 },
                { new Guid("41ef0c07-6131-410e-92bd-3548e6d23f28"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Testowanie", "Testy integracyjne", 12 }
            });

        migrationBuilder.UpdateData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValue: new Guid("1d445487-509b-4bf3-bd4f-839aa9733fea"),
            column: "Name",
            value: "Local LLM Deployment (Ollama)");

        migrationBuilder.UpdateData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValue: new Guid("8984bfae-6e59-4ba2-816e-4e65bc877f06"),
            column: "Name",
            value: "Lokalne wdrażanie LLM (Ollama)");
    }
}
