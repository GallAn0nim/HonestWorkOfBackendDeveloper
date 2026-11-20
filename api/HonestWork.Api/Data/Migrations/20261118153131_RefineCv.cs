using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261118153131_RefineCv : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "WorkPreference",
            table: "CvProfiles",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.UpdateData(
            table: "CvProfiles",
            keyColumn: "Id",
            keyValue: new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"),
            columns: new[] { "Title", "WorkPreference" },
            values: new object[] { "Team Lead | Senior .NET Developer", "Remote preferred" });

        migrationBuilder.UpdateData(
            table: "CvProfiles",
            keyColumn: "Id",
            keyValue: new Guid("81735658-6291-4cb7-accc-32011498c07d"),
            columns: new[] { "Title", "WorkPreference" },
            values: new object[] { "Team Lead | Senior .NET Developer", "Preferowana praca zdalna" });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("470017b4-2bea-4dcc-a8e2-7f363264b5f4"),
            column: "Role",
            value: "Team Lead / Programista");

        migrationBuilder.DeleteData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("91db7e75-5186-4987-b192-b7e572fcbfae"),
                new Guid("8b414631-504a-40a8-a9c2-a5e1378a1ded")
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("82bd2baa-c2a3-4814-b63b-46e21f2f56c7"),
            column: "Highlights",
            value: new[]
            {
                "Architected unified product data and vehicle data platforms integrating multiple data sources across Europe.",
                "Scaled the team from 11 to 21 members (15 internal, 6 external specialists).",
                "Serve as the primary technical contact for client stakeholders, turning business requirements into delivery plans.",
                "Delivered integrations with 15 major ERP systems across multiple European countries.",
                "Led the full rollout of an order management service across Germany, handling ~20 million orders annually.",
                "Manage high-traffic 24/7 production services (35–500 req/s) with p95 under 850 ms on critical endpoints, high availability and zero-downtime deployments."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("f50a9b41-da3b-4fb3-ae9c-d08162afbd0b"),
            column: "Highlights",
            value: new[]
            {
                "Engineered a rule-based product assortment management system for international B2B and B2C automotive platforms.",
                "Built a unified order management and integration platform supporting multiple ERP systems across Europe.",
                "Integrated one of the largest automotive ERP systems in Central and Eastern Europe (CEE).",
                "Helped scale the team from 3 to 11 members while acting as Scrum Master."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("eb43e6c0-7693-4262-8c25-069a9d09e508"),
            column: "Highlights",
            value: new[]
            {
                "Automated pricing with a rule-based system for an international automotive e-commerce platform.",
                "Single-handedly delivered and maintained a Sitefinity CMS solution for the customer returns process in the Benelux automotive market.",
                "Developed an email template generation system supporting multiple email types and sales channels."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("470017b4-2bea-4dcc-a8e2-7f363264b5f4"),
            column: "Highlights",
            value: new[]
            {
                "Zaprojektowałem architekturę ujednoliconych platform danych produktowych i pojazdowych, integrujących wiele źródeł danych z całej Europy.",
                "Rozbudowałem zespół z 11 do 21 osób (15 wewnętrznych, 6 zewnętrznych specjalistów).",
                "Pełnię rolę głównego kontaktu technicznego dla interesariuszy po stronie klienta, przekładając wymagania biznesowe na plany realizacji.",
                "Dostarczyłem integracje z 15 dużymi systemami ERP w wielu krajach Europy.",
                "Kierowałem pełnym wdrożeniem usługi zarządzania zamówieniami w Niemczech, obsługującej ok. 20 mln zamówień rocznie.",
                "Utrzymuję wysokoobciążone usługi produkcyjne 24/7 (35–500 req/s) z p95 poniżej 850 ms dla kluczowych endpointów, wysoką dostępnością i wdrożeniami bez przestojów."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("433d41dc-6a86-4d25-99ff-3a502e0a3600"),
            column: "Highlights",
            value: new[]
            {
                "Opracowałem regułowy system zarządzania asortymentem produktów dla międzynarodowych platform motoryzacyjnych B2B i B2C.",
                "Zbudowałem ujednoliconą platformę zarządzania zamówieniami i integracji, obsługującą wiele systemów ERP w Europie.",
                "Zintegrowałem jeden z największych motoryzacyjnych systemów ERP w Europie Środkowo-Wschodniej (CEE).",
                "Pomogłem rozbudować zespół z 3 do 11 osób, pełniąc jednocześnie rolę Scrum Mastera."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("2214aa78-3923-4498-8871-336ed00bad36"),
            column: "Highlights",
            value: new[]
            {
                "Zautomatyzowałem ustalanie cen za pomocą regułowego systemu dla międzynarodowej platformy e-commerce z branży motoryzacyjnej.",
                "W pojedynkę dostarczyłem i utrzymywałem rozwiązanie w Sitefinity CMS obsługujące proces zwrotów klientów na rynku motoryzacyjnym Beneluksu.",
                "Rozwinąłem system generowania szablonów e-mail obsługujący wiele typów wiadomości i kanałów sprzedaży."
            });

        migrationBuilder.Sql("DELETE FROM \"CvSkills\";");

        migrationBuilder.InsertData(
            table: "CvSkills",
            columns: new[] { "Id", "ProfileId", "Category", "Name", "SortOrder" },
            values: new object[,]
            {
                { new Guid("4daae29b-3681-4678-942c-8be261360add"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Languages & Frameworks", "C#", 0 },
                { new Guid("33794714-cd63-45ca-b0ab-f71f492c3a30"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Languages & Frameworks", ".NET / ASP.NET Core", 1 },
                { new Guid("a9a6e43a-4b8b-4fb8-8472-f49e8fa4c683"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Languages & Frameworks", "Entity Framework Core", 2 },
                { new Guid("7ca02baa-45b5-4d2f-bf27-20924da57ad0"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "REST APIs", 3 },
                { new Guid("c5d4dc82-a609-437e-855f-1dac45454d80"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "gRPC", 4 },
                { new Guid("384ea3db-222f-4e7e-818d-d64c493f3acd"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "Distributed Systems", 5 },
                { new Guid("5b44c9a1-b51a-4a90-8ac6-4459d1adba12"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "Microservices", 6 },
                { new Guid("ddea316d-7675-45e2-bec0-5fb67d4ec17e"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Databases & Caching", "SQL Server", 7 },
                { new Guid("6e858b1c-f60e-4274-a451-afaa9bcd5940"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Databases & Caching", "PostgreSQL", 8 },
                { new Guid("9942da2d-b2d2-4dc5-b94e-5269e7408e98"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Databases & Caching", "Redis", 9 },
                { new Guid("b571e62b-5af7-40b6-a938-c33e093fbd93"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Databases & Caching", "Valkey", 10 },
                { new Guid("8abceff2-e883-4606-9db2-2f6a29f3a275"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Testing", "xUnit", 11 },
                { new Guid("dfffc670-a01a-4270-b534-84394c6d43a9"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Testing", "Integration Tests", 12 },
                { new Guid("b6d0447e-c714-411b-8358-968787af98dd"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Testing", "Load Testing (Locust)", 13 },
                { new Guid("d7b38fb3-d429-40ee-8c6b-3a91f164cbb2"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "AWS", 14 },
                { new Guid("3b975849-5ca6-46c9-b167-3cdb83e1fd74"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "SQS", 15 },
                { new Guid("eb448047-9679-49f8-8b77-711a3ad7d8c4"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "Docker", 16 },
                { new Guid("4bc9652d-d0c4-4e06-999a-e1bb37e20a7d"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "GitLab CI/CD", 17 },
                { new Guid("a05db714-7813-4a3d-b1b2-b31743737fe7"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "Git", 18 },
                { new Guid("07c5e22c-1f0b-4c04-9b1b-49c0319cac90"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "Apigee", 19 },
                { new Guid("867ecc7a-0023-4e8c-a6d3-d5ec6aad3951"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "Datadog", 20 },
                { new Guid("adb6c67a-4e73-4d14-a987-02e857626f45"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Leadership", "Technical Leadership", 21 },
                { new Guid("5aa6d180-e203-4ee6-b719-9eed50fc8cce"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Leadership", "Team Management", 22 },
                { new Guid("e04444d6-686d-470d-905a-49aa0b5e4317"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "JavaScript", 23 },
                { new Guid("c05d803b-947b-4fbe-80f3-0608f808f2cc"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "TypeScript", 24 },
                { new Guid("956fd316-4cee-4ed9-a90a-57c7c5b279d7"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "HTML", 25 },
                { new Guid("0d24e49f-0027-4422-a4b4-42f7b52a6b2a"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "CSS", 26 },
                { new Guid("1bc54a6c-c657-4d9c-a547-4736f7408948"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "LLM Integration", 27 },
                { new Guid("5d5085b4-43c1-49ee-b863-09d971e160ea"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "AI Agents", 28 },
                { new Guid("1d445487-509b-4bf3-bd4f-839aa9733fea"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "Local LLM Deployment (Ollama)", 29 },
                { new Guid("057bed08-e631-4f64-8f96-b04b38a944a1"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "Claude Code", 30 },
                { new Guid("5bff4c03-b5d5-4a00-869c-c6a07e947902"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Języki i frameworki", "C#", 0 },
                { new Guid("d6dcc9c8-f981-45cf-b28d-66b5eebc594d"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Języki i frameworki", ".NET / ASP.NET Core", 1 },
                { new Guid("a8463ff2-ffcf-4c06-93b6-e0b2cfbe509f"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Języki i frameworki", "Entity Framework Core", 2 },
                { new Guid("faae11f6-99cf-4d14-bcfb-fa52aae82904"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "REST APIs", 3 },
                { new Guid("4cfeb273-62d5-4bf1-a50d-1405e23d5b96"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "gRPC", 4 },
                { new Guid("a70018b7-24b7-4a46-be25-1430dfb9dee5"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "Systemy rozproszone", 5 },
                { new Guid("cc891b25-f9b5-4e86-90a7-e24cd6f1d5d6"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "Mikroserwisy", 6 },
                { new Guid("ff2532fd-0104-4023-a6c8-5ab2dea3bc47"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Bazy danych i cache", "SQL Server", 7 },
                { new Guid("50636f6e-f077-4f5d-93b6-b47d2c24edcd"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Bazy danych i cache", "PostgreSQL", 8 },
                { new Guid("3152de1d-ae00-4464-aeef-dc5e6bf84fab"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Bazy danych i cache", "Redis", 9 },
                { new Guid("a64d12f9-3f07-4717-b212-e6da7bd648bd"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Bazy danych i cache", "Valkey", 10 },
                { new Guid("7e147f73-2d5c-421b-b672-164588e846b4"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Testowanie", "xUnit", 11 },
                { new Guid("41ef0c07-6131-410e-92bd-3548e6d23f28"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Testowanie", "Testy integracyjne", 12 },
                { new Guid("72426e43-902b-412b-8468-483b08d073a3"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Testowanie", "Testy obciążeniowe (Locust)", 13 },
                { new Guid("6d8ea0ff-f1f6-4aef-b272-0372949e8281"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "AWS", 14 },
                { new Guid("99e4a56e-4846-4680-b6dc-5e65c5f6427c"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "SQS", 15 },
                { new Guid("f6c765fd-1ebd-4be4-8925-07a2b056935b"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "Docker", 16 },
                { new Guid("bad3830a-f121-4705-ba09-4fd19ef23934"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "GitLab CI/CD", 17 },
                { new Guid("898caf28-c708-48f6-8208-66e499c6c356"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "Git", 18 },
                { new Guid("45e5c567-01a5-471d-9ad4-fb13f2985baa"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "Apigee", 19 },
                { new Guid("e07d63e7-e87e-40e0-b178-72bfa29cff53"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "Datadog", 20 },
                { new Guid("f3c0298a-d23b-47af-962b-f53aaea8e25d"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Przywództwo", "Przywództwo techniczne", 21 },
                { new Guid("65556e26-2236-4cfa-b0aa-e19c204a5963"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Przywództwo", "Zarządzanie zespołem", 22 },
                { new Guid("40d10833-575a-4426-961b-474732f0d1c9"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "JavaScript", 23 },
                { new Guid("922f57f1-b993-4f6d-9aa5-3a388571b980"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "TypeScript", 24 },
                { new Guid("86036651-1b4b-4218-a8a5-280830d0d75e"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "HTML", 25 },
                { new Guid("cf0a167b-43ce-404f-bde9-baffa6d5ef42"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "CSS", 26 },
                { new Guid("4813f5f1-98a2-4728-90e3-31fab7594868"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Integracja LLM", 27 },
                { new Guid("78a14cb9-8ad3-4d17-bdeb-5b8d8ab7a17a"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Agenci AI", 28 },
                { new Guid("8984bfae-6e59-4ba2-816e-4e65bc877f06"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Lokalne wdrażanie LLM (Ollama)", 29 },
                { new Guid("a009fbf3-e27a-4d40-8c0e-efa758c9bc0b"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Claude Code", 30 }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("4daae29b-3681-4678-942c-8be261360add"),
                new Guid("33794714-cd63-45ca-b0ab-f71f492c3a30"),
                new Guid("a9a6e43a-4b8b-4fb8-8472-f49e8fa4c683"),
                new Guid("7ca02baa-45b5-4d2f-bf27-20924da57ad0"),
                new Guid("c5d4dc82-a609-437e-855f-1dac45454d80"),
                new Guid("384ea3db-222f-4e7e-818d-d64c493f3acd"),
                new Guid("5b44c9a1-b51a-4a90-8ac6-4459d1adba12"),
                new Guid("ddea316d-7675-45e2-bec0-5fb67d4ec17e"),
                new Guid("6e858b1c-f60e-4274-a451-afaa9bcd5940"),
                new Guid("9942da2d-b2d2-4dc5-b94e-5269e7408e98"),
                new Guid("b571e62b-5af7-40b6-a938-c33e093fbd93"),
                new Guid("8abceff2-e883-4606-9db2-2f6a29f3a275"),
                new Guid("dfffc670-a01a-4270-b534-84394c6d43a9"),
                new Guid("b6d0447e-c714-411b-8358-968787af98dd"),
                new Guid("d7b38fb3-d429-40ee-8c6b-3a91f164cbb2"),
                new Guid("3b975849-5ca6-46c9-b167-3cdb83e1fd74"),
                new Guid("eb448047-9679-49f8-8b77-711a3ad7d8c4"),
                new Guid("4bc9652d-d0c4-4e06-999a-e1bb37e20a7d"),
                new Guid("a05db714-7813-4a3d-b1b2-b31743737fe7"),
                new Guid("07c5e22c-1f0b-4c04-9b1b-49c0319cac90"),
                new Guid("867ecc7a-0023-4e8c-a6d3-d5ec6aad3951"),
                new Guid("adb6c67a-4e73-4d14-a987-02e857626f45"),
                new Guid("5aa6d180-e203-4ee6-b719-9eed50fc8cce"),
                new Guid("e04444d6-686d-470d-905a-49aa0b5e4317"),
                new Guid("c05d803b-947b-4fbe-80f3-0608f808f2cc"),
                new Guid("956fd316-4cee-4ed9-a90a-57c7c5b279d7"),
                new Guid("0d24e49f-0027-4422-a4b4-42f7b52a6b2a"),
                new Guid("1bc54a6c-c657-4d9c-a547-4736f7408948"),
                new Guid("5d5085b4-43c1-49ee-b863-09d971e160ea"),
                new Guid("1d445487-509b-4bf3-bd4f-839aa9733fea"),
                new Guid("057bed08-e631-4f64-8f96-b04b38a944a1"),
                new Guid("5bff4c03-b5d5-4a00-869c-c6a07e947902"),
                new Guid("d6dcc9c8-f981-45cf-b28d-66b5eebc594d"),
                new Guid("a8463ff2-ffcf-4c06-93b6-e0b2cfbe509f"),
                new Guid("faae11f6-99cf-4d14-bcfb-fa52aae82904"),
                new Guid("4cfeb273-62d5-4bf1-a50d-1405e23d5b96"),
                new Guid("a70018b7-24b7-4a46-be25-1430dfb9dee5"),
                new Guid("cc891b25-f9b5-4e86-90a7-e24cd6f1d5d6"),
                new Guid("ff2532fd-0104-4023-a6c8-5ab2dea3bc47"),
                new Guid("50636f6e-f077-4f5d-93b6-b47d2c24edcd"),
                new Guid("3152de1d-ae00-4464-aeef-dc5e6bf84fab"),
                new Guid("a64d12f9-3f07-4717-b212-e6da7bd648bd"),
                new Guid("7e147f73-2d5c-421b-b672-164588e846b4"),
                new Guid("41ef0c07-6131-410e-92bd-3548e6d23f28"),
                new Guid("72426e43-902b-412b-8468-483b08d073a3"),
                new Guid("6d8ea0ff-f1f6-4aef-b272-0372949e8281"),
                new Guid("99e4a56e-4846-4680-b6dc-5e65c5f6427c"),
                new Guid("f6c765fd-1ebd-4be4-8925-07a2b056935b"),
                new Guid("bad3830a-f121-4705-ba09-4fd19ef23934"),
                new Guid("898caf28-c708-48f6-8208-66e499c6c356"),
                new Guid("45e5c567-01a5-471d-9ad4-fb13f2985baa"),
                new Guid("e07d63e7-e87e-40e0-b178-72bfa29cff53"),
                new Guid("f3c0298a-d23b-47af-962b-f53aaea8e25d"),
                new Guid("65556e26-2236-4cfa-b0aa-e19c204a5963"),
                new Guid("40d10833-575a-4426-961b-474732f0d1c9"),
                new Guid("922f57f1-b993-4f6d-9aa5-3a388571b980"),
                new Guid("86036651-1b4b-4218-a8a5-280830d0d75e"),
                new Guid("cf0a167b-43ce-404f-bde9-baffa6d5ef42"),
                new Guid("4813f5f1-98a2-4728-90e3-31fab7594868"),
                new Guid("78a14cb9-8ad3-4d17-bdeb-5b8d8ab7a17a"),
                new Guid("8984bfae-6e59-4ba2-816e-4e65bc877f06"),
                new Guid("a009fbf3-e27a-4d40-8c0e-efa758c9bc0b")
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("82bd2baa-c2a3-4814-b63b-46e21f2f56c7"),
            column: "Highlights",
            value: new[]
            {
                "Designed and developed unified product data and vehicle data platforms integrating multiple data sources across Europe.",
                "Scaled the team from 11 to 21 members (15 internal, 6 external specialists).",
                "Serve as the primary technical contact for client stakeholders, turning business requirements into delivery plans.",
                "Delivered integrations with 15 major ERP systems across multiple European countries.",
                "Led the full rollout of an order management service across Germany, handling ~20 million orders annually.",
                "Manage high-traffic 24/7 production services (35–500 req/s) with p95 under 850 ms on critical endpoints, high availability and zero-downtime deployments."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("f50a9b41-da3b-4fb3-ae9c-d08162afbd0b"),
            column: "Highlights",
            value: new[]
            {
                "Designed and developed a rule-based product assortment management system for international B2B and B2C automotive platforms.",
                "Built a unified order management and integration platform supporting multiple ERP systems across Europe.",
                "Integrated one of the largest automotive ERP systems in Central and Eastern Europe (CEE).",
                "Helped scale the team from 3 to 11 members while acting as Scrum Master."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("eb43e6c0-7693-4262-8c25-069a9d09e508"),
            column: "Highlights",
            value: new[]
            {
                "Designed and developed a rule-based pricing automation system for an international automotive e-commerce platform.",
                "Solely designed, developed and maintained a Sitefinity CMS solution for the customer returns process in the Benelux automotive market.",
                "Developed an email template generation system supporting multiple email types and sales channels."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("470017b4-2bea-4dcc-a8e2-7f363264b5f4"),
            column: "Highlights",
            value: new[]
            {
                "Zaprojektowałem i rozwinąłem ujednolicone platformy danych produktowych i pojazdowych, integrujące wiele źródeł danych z całej Europy.",
                "Rozbudowałem zespół z 11 do 21 osób (15 wewnętrznych, 6 zewnętrznych specjalistów).",
                "Pełnię rolę głównego kontaktu technicznego dla interesariuszy po stronie klienta, przekładając wymagania biznesowe na plany realizacji.",
                "Dostarczyłem integracje z 15 dużymi systemami ERP w wielu krajach Europy.",
                "Kierowałem pełnym wdrożeniem usługi zarządzania zamówieniami w Niemczech, obsługującej ok. 20 mln zamówień rocznie.",
                "Utrzymuję wysokoobciążone usługi produkcyjne 24/7 (35–500 req/s) z p95 poniżej 850 ms dla kluczowych endpointów, wysoką dostępnością i wdrożeniami bez przestojów."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("433d41dc-6a86-4d25-99ff-3a502e0a3600"),
            column: "Highlights",
            value: new[]
            {
                "Zaprojektowałem i rozwinąłem regułowy system zarządzania asortymentem produktów dla międzynarodowych platform motoryzacyjnych B2B i B2C.",
                "Zbudowałem ujednoliconą platformę zarządzania zamówieniami i integracji, obsługującą wiele systemów ERP w Europie.",
                "Zintegrowałem jeden z największych motoryzacyjnych systemów ERP w Europie Środkowo-Wschodniej (CEE).",
                "Pomogłem rozbudować zespół z 3 do 11 osób, pełniąc jednocześnie rolę Scrum Mastera."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("2214aa78-3923-4498-8871-336ed00bad36"),
            column: "Highlights",
            value: new[]
            {
                "Zaprojektowałem i rozwinąłem regułowy system automatyzacji cen dla międzynarodowej platformy e-commerce z branży motoryzacyjnej.",
                "Samodzielnie zaprojektowałem, rozwinąłem i utrzymywałem rozwiązanie w Sitefinity CMS obsługujące proces zwrotów klientów na rynku motoryzacyjnym Beneluksu.",
                "Rozwinąłem system generowania szablonów e-mail obsługujący wiele typów wiadomości i kanałów sprzedaży."
            });

        migrationBuilder.InsertData(
            table: "CvEducations",
            columns: new[] { "Id", "ProfileId", "Institution", "Degree", "Field", "Thesis", "StartDate", "EndDate", "SortOrder" },
            values: new object[,]
            {
                { new Guid("91db7e75-5186-4987-b192-b7e572fcbfae"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Zespół Szkół Ponadgimnazjalnych w Białogardzie", "Technikum informatyczne", null, null, new DateOnly(2009, 9, 1), new DateOnly(2013, 4, 26), 1 },
                { new Guid("8b414631-504a-40a8-a9c2-a5e1378a1ded"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Zespół Szkół Ponadgimnazjalnych in Białogard", "Technical High School in Białogard", "IT technician", null, new DateOnly(2009, 9, 1), new DateOnly(2013, 4, 26), 1 }
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("470017b4-2bea-4dcc-a8e2-7f363264b5f4"),
            column: "Role",
            value: "Team Leader / Programista");

        migrationBuilder.UpdateData(
            table: "CvProfiles",
            keyColumn: "Id",
            keyValue: new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"),
            columns: new[] { "Title" },
            values: new object[] { "Tech Lead | Senior .NET Developer" });

        migrationBuilder.UpdateData(
            table: "CvProfiles",
            keyColumn: "Id",
            keyValue: new Guid("81735658-6291-4cb7-accc-32011498c07d"),
            columns: new[] { "Title" },
            values: new object[] { "Tech Lead | Senior .NET Developer" });

        migrationBuilder.DropColumn(
            name: "WorkPreference",
            table: "CvProfiles");
    }
}
