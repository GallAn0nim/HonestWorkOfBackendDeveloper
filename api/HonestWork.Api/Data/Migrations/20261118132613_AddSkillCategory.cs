using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261118132613_AddSkillCategory : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Category",
            table: "CvSkills",
            type: "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.InsertData(
            table: "CvSkills",
            columns: new[] { "Id", "ProfileId", "Category", "Name", "SortOrder" },
            values: new object[,]
            {
                { new Guid("2052052a-6d40-46d0-b5c7-c9e8855dda75"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Languages & Frameworks", "C#", 0 },
                { new Guid("33d2f049-42a7-433d-9067-b20ff9714baf"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Languages & Frameworks", ".NET / ASP.NET Core", 1 },
                { new Guid("b3b88c85-5367-4e5f-a1f0-047a65363c5e"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Languages & Frameworks", "Entity Framework Core", 2 },
                { new Guid("246c0cbe-ba10-4139-8879-1f7f27ff73fb"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "REST APIs", 3 },
                { new Guid("de790857-742c-4c9e-b7a8-bd7f260dd17e"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "gRPC", 4 },
                { new Guid("cae41187-4929-44c8-bd1a-2d39d5a7a0a1"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "Distributed Systems", 5 },
                { new Guid("0235a1ea-d37e-4d00-b1a1-fb4a847309b1"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Architecture & Backend", "Microservices", 6 },
                { new Guid("38e3cd71-50b5-454a-8da4-6f7beb0a399d"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "System Design", "Databases & Caching", 7 },
                { new Guid("f3a25a5f-58a3-402d-bd87-d208841a53e1"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "System Design", "Microsoft SQL Server", 8 },
                { new Guid("f67a8e65-5b2c-44f5-a65c-e3672a60adc6"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "System Design", "PostgreSQL", 9 },
                { new Guid("2bf8ea39-2a08-46b5-98d3-3507982ddc45"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "System Design", "Redis / Valkey", 10 },
                { new Guid("a02588c1-d3d0-4d6e-a76c-6543070225b5"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "AWS", 11 },
                { new Guid("d0a8ee25-1b1a-4d3e-a189-a63684087100"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "GitLab CI/CD", 12 },
                { new Guid("bf9791d4-b7a4-400d-ac70-8ea00f2fdfff"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "Git", 13 },
                { new Guid("f26a8f5b-6edd-4c56-b497-a5fa44e42dfc"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "Apigee", 14 },
                { new Guid("b32767aa-c120-498b-948f-ec7d4b469356"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Monitoring", "Datadog", 15 },
                { new Guid("738fea85-dc98-4c6d-95f9-602f111f4199"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Monitoring", "Logging & Monitoring", 16 },
                { new Guid("66725f09-ed90-458f-ae2c-e6b014c7c3a4"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Leadership", "Technical Leadership", 17 },
                { new Guid("85e4cda4-836a-49e9-a37b-2e9826a6a79e"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Leadership", "Team Management", 18 },
                { new Guid("05005008-350b-4651-9122-653d86e38152"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Leadership", "Code Reviews", 19 },
                { new Guid("5a9d4f80-2ad2-48d3-aa45-e1a24c514ccd"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "JavaScript", 20 },
                { new Guid("76c4dffa-7529-44b0-ba60-b20c45affa91"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "TypeScript", 21 },
                { new Guid("8b90691f-d8d3-42da-845a-63c731d65bb3"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "HTML5 / CSS3", 22 },
                { new Guid("1cf7fbca-0681-45d5-b1e7-f111a84b34bc"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "REST API Integration", 23 },
                { new Guid("f8be9056-c8d7-4b9f-b7ee-ddc1b74a92e1"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Frontend", "Responsive Web Design", 24 },
                { new Guid("ca1b8dde-c23c-4a69-8a8d-7cc747d41abf"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Języki i frameworki", "C#", 0 },
                { new Guid("5d82c09a-b0a1-4265-9433-76c242d34b39"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Języki i frameworki", ".NET / ASP.NET Core", 1 },
                { new Guid("fd5079ce-2df3-46b4-a738-01c36e4a7806"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Języki i frameworki", "Entity Framework Core", 2 },
                { new Guid("b28043cf-8dce-4afc-914b-e03e654b82b4"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "REST APIs", 3 },
                { new Guid("c1dfa39f-ca9a-48c6-96aa-708fe3a047dd"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "gRPC", 4 },
                { new Guid("111c7632-6b60-4bba-84ad-5b17365fbcc3"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "Systemy rozproszone", 5 },
                { new Guid("83b9ce07-007a-4430-a29d-a4bc95f8385b"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Architektura i backend", "Mikroserwisy", 6 },
                { new Guid("493b4a03-0544-4f27-85ca-43e20f70adba"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Projektowanie systemów", "Bazy danych i cache", 7 },
                { new Guid("afe8efce-470a-4b94-8e6a-a0f5a523be77"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Projektowanie systemów", "Microsoft SQL Server", 8 },
                { new Guid("ca813d09-c2da-4607-9c39-daa530326cc4"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Projektowanie systemów", "PostgreSQL", 9 },
                { new Guid("cbb1df59-f39e-4026-ac05-3a46803bdaf9"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Projektowanie systemów", "Redis / Valkey", 10 },
                { new Guid("e14a4484-defe-40c3-8113-fa57f78db140"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "AWS", 11 },
                { new Guid("e2003ff9-35df-483d-b553-93d753fb7937"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "GitLab CI/CD", 12 },
                { new Guid("16c45300-db89-4a7a-b1fb-8b4e516fc4ee"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "Git", 13 },
                { new Guid("63a237ad-a430-41e9-b3b3-b086ba869902"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "Apigee", 14 },
                { new Guid("ee084f1b-bdcb-43fa-ab0a-3300666fb6f6"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Monitoring", "Datadog", 15 },
                { new Guid("60a84faf-9333-434c-8f8f-c04b84707ccf"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Monitoring", "Logowanie i monitoring", 16 },
                { new Guid("d2896df6-a265-4e95-8c3e-0c67519afee7"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Przywództwo", "Przywództwo techniczne", 17 },
                { new Guid("5b222944-186a-4f55-9f4a-d95932d24872"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Przywództwo", "Zarządzanie zespołem", 18 },
                { new Guid("d0cb278a-c599-493b-9a39-78ed42b4ad4c"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Przywództwo", "Code review", 19 },
                { new Guid("28ed3e12-f103-498e-a4d7-bfa661c75cc7"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "JavaScript", 20 },
                { new Guid("a0463156-8f32-45ee-a161-cad58ca5a28d"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "TypeScript", 21 },
                { new Guid("a078745b-85b0-4d1a-b184-64aa22f85cd3"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "HTML5 / CSS3", 22 },
                { new Guid("2cbf95c2-09ba-45b7-a759-e2c39aa78224"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "Integracja z REST API", 23 },
                { new Guid("e6ed12bf-80cd-4f9b-a71a-b35d945a30c8"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Frontend", "Responsywne strony WWW", 24 }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("2052052a-6d40-46d0-b5c7-c9e8855dda75"),
                new Guid("33d2f049-42a7-433d-9067-b20ff9714baf"),
                new Guid("b3b88c85-5367-4e5f-a1f0-047a65363c5e"),
                new Guid("246c0cbe-ba10-4139-8879-1f7f27ff73fb"),
                new Guid("de790857-742c-4c9e-b7a8-bd7f260dd17e"),
                new Guid("cae41187-4929-44c8-bd1a-2d39d5a7a0a1"),
                new Guid("0235a1ea-d37e-4d00-b1a1-fb4a847309b1"),
                new Guid("38e3cd71-50b5-454a-8da4-6f7beb0a399d"),
                new Guid("f3a25a5f-58a3-402d-bd87-d208841a53e1"),
                new Guid("f67a8e65-5b2c-44f5-a65c-e3672a60adc6"),
                new Guid("2bf8ea39-2a08-46b5-98d3-3507982ddc45"),
                new Guid("a02588c1-d3d0-4d6e-a76c-6543070225b5"),
                new Guid("d0a8ee25-1b1a-4d3e-a189-a63684087100"),
                new Guid("bf9791d4-b7a4-400d-ac70-8ea00f2fdfff"),
                new Guid("f26a8f5b-6edd-4c56-b497-a5fa44e42dfc"),
                new Guid("b32767aa-c120-498b-948f-ec7d4b469356"),
                new Guid("738fea85-dc98-4c6d-95f9-602f111f4199"),
                new Guid("66725f09-ed90-458f-ae2c-e6b014c7c3a4"),
                new Guid("85e4cda4-836a-49e9-a37b-2e9826a6a79e"),
                new Guid("05005008-350b-4651-9122-653d86e38152"),
                new Guid("5a9d4f80-2ad2-48d3-aa45-e1a24c514ccd"),
                new Guid("76c4dffa-7529-44b0-ba60-b20c45affa91"),
                new Guid("8b90691f-d8d3-42da-845a-63c731d65bb3"),
                new Guid("1cf7fbca-0681-45d5-b1e7-f111a84b34bc"),
                new Guid("f8be9056-c8d7-4b9f-b7ee-ddc1b74a92e1"),
                new Guid("ca1b8dde-c23c-4a69-8a8d-7cc747d41abf"),
                new Guid("5d82c09a-b0a1-4265-9433-76c242d34b39"),
                new Guid("fd5079ce-2df3-46b4-a738-01c36e4a7806"),
                new Guid("b28043cf-8dce-4afc-914b-e03e654b82b4"),
                new Guid("c1dfa39f-ca9a-48c6-96aa-708fe3a047dd"),
                new Guid("111c7632-6b60-4bba-84ad-5b17365fbcc3"),
                new Guid("83b9ce07-007a-4430-a29d-a4bc95f8385b"),
                new Guid("493b4a03-0544-4f27-85ca-43e20f70adba"),
                new Guid("afe8efce-470a-4b94-8e6a-a0f5a523be77"),
                new Guid("ca813d09-c2da-4607-9c39-daa530326cc4"),
                new Guid("cbb1df59-f39e-4026-ac05-3a46803bdaf9"),
                new Guid("e14a4484-defe-40c3-8113-fa57f78db140"),
                new Guid("e2003ff9-35df-483d-b553-93d753fb7937"),
                new Guid("16c45300-db89-4a7a-b1fb-8b4e516fc4ee"),
                new Guid("63a237ad-a430-41e9-b3b3-b086ba869902"),
                new Guid("ee084f1b-bdcb-43fa-ab0a-3300666fb6f6"),
                new Guid("60a84faf-9333-434c-8f8f-c04b84707ccf"),
                new Guid("d2896df6-a265-4e95-8c3e-0c67519afee7"),
                new Guid("5b222944-186a-4f55-9f4a-d95932d24872"),
                new Guid("d0cb278a-c599-493b-9a39-78ed42b4ad4c"),
                new Guid("28ed3e12-f103-498e-a4d7-bfa661c75cc7"),
                new Guid("a0463156-8f32-45ee-a161-cad58ca5a28d"),
                new Guid("a078745b-85b0-4d1a-b184-64aa22f85cd3"),
                new Guid("2cbf95c2-09ba-45b7-a759-e2c39aa78224"),
                new Guid("e6ed12bf-80cd-4f9b-a71a-b35d945a30c8")
            });

        migrationBuilder.DropColumn(
            name: "Category",
            table: "CvSkills");
    }
}
