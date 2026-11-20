using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261118150957_UpdateCvSkillsAndTitle : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Title",
            table: "CvProfiles",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.Sql("UPDATE \"CvProfiles\" SET \"Title\" = 'Tech Lead | Senior .NET Developer';");
        migrationBuilder.Sql("DELETE FROM \"CvSkills\";");

        migrationBuilder.InsertData(
            table: "CvSkills",
            columns: new[] { "Id", "ProfileId", "Category", "Name", "SortOrder" },
            values: new object[,]
            {
                { new Guid("fa8446fd-8bf5-487f-af29-bd6d6257f28f"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Languages & Frameworks", "C#", 0 },
                { new Guid("cba239d9-6814-42e2-ac3a-984891965b88"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Languages & Frameworks", ".NET / ASP.NET Core", 1 },
                { new Guid("b91e8290-7cd4-4d37-8d02-c5e94871a4f5"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Languages & Frameworks", "Entity Framework Core", 2 },
                { new Guid("1d447998-dd9b-4991-bcaa-b00af3f3d56b"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "REST APIs", 3 },
                { new Guid("cae4cae1-35e2-4175-a634-8f724da81930"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "gRPC", 4 },
                { new Guid("337e29c6-bf07-43a2-8fd4-66cf4afee575"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "Distributed Systems", 5 },
                { new Guid("749bf109-c255-4ced-bd4e-ab2279d6b368"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "Microservices", 6 },
                { new Guid("ff6f96c7-9824-4f3f-b791-05c49591f988"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Databases & Caching", "SQL Server", 7 },
                { new Guid("6b724781-88f8-4995-9357-993f4e0ea548"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Databases & Caching", "PostgreSQL", 8 },
                { new Guid("63c45dff-963b-45c7-ad79-7e0d05f3150f"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Databases & Caching", "Redis", 9 },
                { new Guid("bd85576a-6527-4b9c-b7ad-52dba337bf3c"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Databases & Caching", "Valkey", 10 },
                { new Guid("05525081-b6e0-4d08-8e55-1d98e526b5fc"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "AWS", 11 },
                { new Guid("bcf79311-a83f-4717-91f0-c5380c979131"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "GitLab CI/CD", 12 },
                { new Guid("80957eb9-d03e-4d1e-83e9-2c9ae0cf08a2"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "Git", 13 },
                { new Guid("e10d6454-5f67-4e9f-803a-cd14c5ada9c5"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "Apigee", 14 },
                { new Guid("a1870a18-aabd-4e52-b777-04812fdd6ba8"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "Datadog", 15 },
                { new Guid("00047633-3dc8-45e3-9e6e-17012cdcb614"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Leadership", "Technical Leadership", 16 },
                { new Guid("709b96c3-6889-4e6d-a979-d9f3a5f30bb3"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Leadership", "Team Management", 17 },
                { new Guid("00513ebb-d924-495c-ad61-75102b64c758"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Leadership", "Code Reviews", 18 },
                { new Guid("0063061e-d44c-4d2d-8821-42c2163ae048"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "JavaScript", 19 },
                { new Guid("dfbee7f9-f73b-4e5e-b46c-a2af9f266770"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "TypeScript", 20 },
                { new Guid("88f956d2-1721-4cb0-8bf9-71b3f3f8e577"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "HTML", 21 },
                { new Guid("1d06bc42-9198-4329-95ec-1dc5d724210c"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "CSS", 22 },
                { new Guid("1b9a5c34-51cc-40da-a961-b01539e6e4aa"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "LLM Integration", 23 },
                { new Guid("d54784dc-0a51-4865-b84b-149a32ccd30e"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "AI Agents", 24 },
                { new Guid("34c1afd9-7bec-4190-b8b8-3cd6d8d9beb0"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "Local LLM Deployment (Ollama)", 25 },
                { new Guid("93515176-b2d9-4daa-b612-51278e2c7f6e"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "AI & LLM", "Claude Code", 26 },
                { new Guid("732562b7-ae58-4524-ba64-d6e7f3412327"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Języki i frameworki", "C#", 0 },
                { new Guid("e5e6af17-ceae-44b8-b6c9-f1cbe89c76f0"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Języki i frameworki", ".NET / ASP.NET Core", 1 },
                { new Guid("2403ae43-0b4f-4ef7-a832-cec4cf519f2f"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Języki i frameworki", "Entity Framework Core", 2 },
                { new Guid("81069683-fbb4-4df2-95da-74e562cc0b14"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "REST APIs", 3 },
                { new Guid("3ce148a8-8421-4f51-aa5d-9db1c16dbe22"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "gRPC", 4 },
                { new Guid("d74cae8a-22f1-454e-a69a-1959076d46ed"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "Systemy rozproszone", 5 },
                { new Guid("2c4f93bb-7d78-4a17-85e2-9615d6c16c53"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "Mikroserwisy", 6 },
                { new Guid("45c329dc-8d53-46d2-be1d-8242cc2a0ff3"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Bazy danych i cache", "SQL Server", 7 },
                { new Guid("acd31fca-fe96-4d93-a2ad-7e23fc1be216"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Bazy danych i cache", "PostgreSQL", 8 },
                { new Guid("1898937f-89e6-4aa6-bc4a-8bc16071e9b0"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Bazy danych i cache", "Redis", 9 },
                { new Guid("96d5ec1f-000b-409f-99c0-26e827478661"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Bazy danych i cache", "Valkey", 10 },
                { new Guid("c2c9912f-0d8a-46d4-af24-947feb6cf859"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "AWS", 11 },
                { new Guid("c3f6c2b5-d0e2-4bb2-b83e-d7279baaebe1"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "GitLab CI/CD", 12 },
                { new Guid("72443517-c795-4710-a9f8-5bb96f5d87fd"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "Git", 13 },
                { new Guid("f8f9158b-32de-421d-9a65-ddf15a5a9a83"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "Apigee", 14 },
                { new Guid("e185dd82-bf6b-4198-88eb-3cb27bd1932a"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "Datadog", 15 },
                { new Guid("86927021-b5f4-465f-b874-bb5e51ff8813"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Przywództwo", "Przywództwo techniczne", 16 },
                { new Guid("128f5157-0d96-4c22-856b-faa5352a832e"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Przywództwo", "Zarządzanie zespołem", 17 },
                { new Guid("15d375ac-02a5-4bb2-8117-bbd540857d2f"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Przywództwo", "Code review", 18 },
                { new Guid("13be63ef-6158-4c3b-b0dc-d762fb516fbb"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "JavaScript", 19 },
                { new Guid("89577576-45a2-4071-8d1e-03d708a03f90"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "TypeScript", 20 },
                { new Guid("005e895e-be0d-4f8a-a5c7-39ef577c4581"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "HTML", 21 },
                { new Guid("618cac4d-a2f8-4beb-ac3a-e57d574fe7ae"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "CSS", 22 },
                { new Guid("90b70fda-6a1e-48d5-a785-a02ed194208f"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Integracja LLM", 23 },
                { new Guid("9fadf75a-1cca-4721-ba00-2ef64e3f1aff"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Agenci AI", 24 },
                { new Guid("7323611b-4274-49fa-abba-6e63da54eec1"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Lokalne wdrażanie LLM (Ollama)", 25 },
                { new Guid("b10cd51d-baa3-474a-b7d5-d7a207174df6"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "AI i LLM", "Claude Code", 26 }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("fa8446fd-8bf5-487f-af29-bd6d6257f28f"),
                new Guid("cba239d9-6814-42e2-ac3a-984891965b88"),
                new Guid("b91e8290-7cd4-4d37-8d02-c5e94871a4f5"),
                new Guid("1d447998-dd9b-4991-bcaa-b00af3f3d56b"),
                new Guid("cae4cae1-35e2-4175-a634-8f724da81930"),
                new Guid("337e29c6-bf07-43a2-8fd4-66cf4afee575"),
                new Guid("749bf109-c255-4ced-bd4e-ab2279d6b368"),
                new Guid("ff6f96c7-9824-4f3f-b791-05c49591f988"),
                new Guid("6b724781-88f8-4995-9357-993f4e0ea548"),
                new Guid("63c45dff-963b-45c7-ad79-7e0d05f3150f"),
                new Guid("bd85576a-6527-4b9c-b7ad-52dba337bf3c"),
                new Guid("05525081-b6e0-4d08-8e55-1d98e526b5fc"),
                new Guid("bcf79311-a83f-4717-91f0-c5380c979131"),
                new Guid("80957eb9-d03e-4d1e-83e9-2c9ae0cf08a2"),
                new Guid("e10d6454-5f67-4e9f-803a-cd14c5ada9c5"),
                new Guid("a1870a18-aabd-4e52-b777-04812fdd6ba8"),
                new Guid("00047633-3dc8-45e3-9e6e-17012cdcb614"),
                new Guid("709b96c3-6889-4e6d-a979-d9f3a5f30bb3"),
                new Guid("00513ebb-d924-495c-ad61-75102b64c758"),
                new Guid("0063061e-d44c-4d2d-8821-42c2163ae048"),
                new Guid("dfbee7f9-f73b-4e5e-b46c-a2af9f266770"),
                new Guid("88f956d2-1721-4cb0-8bf9-71b3f3f8e577"),
                new Guid("1d06bc42-9198-4329-95ec-1dc5d724210c"),
                new Guid("1b9a5c34-51cc-40da-a961-b01539e6e4aa"),
                new Guid("d54784dc-0a51-4865-b84b-149a32ccd30e"),
                new Guid("34c1afd9-7bec-4190-b8b8-3cd6d8d9beb0"),
                new Guid("93515176-b2d9-4daa-b612-51278e2c7f6e"),
                new Guid("732562b7-ae58-4524-ba64-d6e7f3412327"),
                new Guid("e5e6af17-ceae-44b8-b6c9-f1cbe89c76f0"),
                new Guid("2403ae43-0b4f-4ef7-a832-cec4cf519f2f"),
                new Guid("81069683-fbb4-4df2-95da-74e562cc0b14"),
                new Guid("3ce148a8-8421-4f51-aa5d-9db1c16dbe22"),
                new Guid("d74cae8a-22f1-454e-a69a-1959076d46ed"),
                new Guid("2c4f93bb-7d78-4a17-85e2-9615d6c16c53"),
                new Guid("45c329dc-8d53-46d2-be1d-8242cc2a0ff3"),
                new Guid("acd31fca-fe96-4d93-a2ad-7e23fc1be216"),
                new Guid("1898937f-89e6-4aa6-bc4a-8bc16071e9b0"),
                new Guid("96d5ec1f-000b-409f-99c0-26e827478661"),
                new Guid("c2c9912f-0d8a-46d4-af24-947feb6cf859"),
                new Guid("c3f6c2b5-d0e2-4bb2-b83e-d7279baaebe1"),
                new Guid("72443517-c795-4710-a9f8-5bb96f5d87fd"),
                new Guid("f8f9158b-32de-421d-9a65-ddf15a5a9a83"),
                new Guid("e185dd82-bf6b-4198-88eb-3cb27bd1932a"),
                new Guid("86927021-b5f4-465f-b874-bb5e51ff8813"),
                new Guid("128f5157-0d96-4c22-856b-faa5352a832e"),
                new Guid("15d375ac-02a5-4bb2-8117-bbd540857d2f"),
                new Guid("13be63ef-6158-4c3b-b0dc-d762fb516fbb"),
                new Guid("89577576-45a2-4071-8d1e-03d708a03f90"),
                new Guid("005e895e-be0d-4f8a-a5c7-39ef577c4581"),
                new Guid("618cac4d-a2f8-4beb-ac3a-e57d574fe7ae"),
                new Guid("90b70fda-6a1e-48d5-a785-a02ed194208f"),
                new Guid("9fadf75a-1cca-4721-ba00-2ef64e3f1aff"),
                new Guid("7323611b-4274-49fa-abba-6e63da54eec1"),
                new Guid("b10cd51d-baa3-474a-b7d5-d7a207174df6")
            });

        migrationBuilder.DropColumn(
            name: "Title",
            table: "CvProfiles");
    }
}
