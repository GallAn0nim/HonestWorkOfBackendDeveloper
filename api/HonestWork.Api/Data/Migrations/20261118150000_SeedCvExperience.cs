using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261118150000_SeedCvExperience : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "CvExperiences",
            columns: new[] { "Id", "ProfileId", "Company", "Role", "StartDate", "EndDate", "Highlights", "SortOrder" },
            values: new object[,]
            {
                {
                    new Guid("82bd2baa-c2a3-4814-b63b-46e21f2f56c7"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "i4B Sp. z o.o.", "Team Lead / Software Developer", new DateOnly(2024, 2, 1), null,
                    new[]
                    {
                        "Designed and developed unified product data and vehicle data platforms integrating multiple data sources across Europe.",
                        "Scaled the team from 11 to 21 members (15 internal, 6 external specialists).",
                        "Serve as the primary technical contact for client stakeholders, turning business requirements into delivery plans.",
                        "Delivered integrations with 15 major ERP systems across multiple European countries.",
                        "Led the full rollout of an order management service across Germany, handling ~20 million orders annually.",
                        "Manage high-traffic 24/7 production services (35–500 req/s) with p95 under 850 ms on critical endpoints, high availability and zero-downtime deployments."
                    },
                    0
                },
                {
                    new Guid("f50a9b41-da3b-4fb3-ae9c-d08162afbd0b"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "i4B Sp. z o.o.", "Software Developer / Scrum Master", new DateOnly(2019, 9, 1), new DateOnly(2024, 2, 1),
                    new[]
                    {
                        "Designed and developed a rule-based product assortment management system for international B2B and B2C automotive platforms.",
                        "Built a unified order management and integration platform supporting multiple ERP systems across Europe.",
                        "Integrated one of the largest automotive ERP systems in Central and Eastern Europe (CEE).",
                        "Helped scale the team from 3 to 11 members while acting as Scrum Master."
                    },
                    1
                },
                {
                    new Guid("eb43e6c0-7693-4262-8c25-069a9d09e508"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "i4B Sp. z o.o.", "Software Developer", new DateOnly(2016, 6, 1), new DateOnly(2019, 9, 1),
                    new[]
                    {
                        "Designed and developed a rule-based pricing automation system for an international automotive e-commerce platform.",
                        "Solely designed, developed and maintained a Sitefinity CMS solution for the customer returns process in the Benelux automotive market.",
                        "Developed an email template generation system supporting multiple email types and sales channels."
                    },
                    2
                },
                {
                    new Guid("470017b4-2bea-4dcc-a8e2-7f363264b5f4"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "i4B Sp. z o.o.", "Team Leader / Programista", new DateOnly(2024, 2, 1), null,
                    new[]
                    {
                        "Zaprojektowałem i rozwinąłem ujednolicone platformy danych produktowych i pojazdowych, integrujące wiele źródeł danych z całej Europy.",
                        "Rozbudowałem zespół z 11 do 21 osób (15 wewnętrznych, 6 zewnętrznych specjalistów).",
                        "Pełnię rolę głównego kontaktu technicznego dla interesariuszy po stronie klienta, przekładając wymagania biznesowe na plany realizacji.",
                        "Dostarczyłem integracje z 15 dużymi systemami ERP w wielu krajach Europy.",
                        "Kierowałem pełnym wdrożeniem usługi zarządzania zamówieniami w Niemczech, obsługującej ok. 20 mln zamówień rocznie.",
                        "Utrzymuję wysokoobciążone usługi produkcyjne 24/7 (35–500 req/s) z p95 poniżej 850 ms dla kluczowych endpointów, wysoką dostępnością i wdrożeniami bez przestojów."
                    },
                    0
                },
                {
                    new Guid("433d41dc-6a86-4d25-99ff-3a502e0a3600"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "i4B Sp. z o.o.", "Programista / Scrum Master", new DateOnly(2019, 9, 1), new DateOnly(2024, 2, 1),
                    new[]
                    {
                        "Zaprojektowałem i rozwinąłem regułowy system zarządzania asortymentem produktów dla międzynarodowych platform motoryzacyjnych B2B i B2C.",
                        "Zbudowałem ujednoliconą platformę zarządzania zamówieniami i integracji, obsługującą wiele systemów ERP w Europie.",
                        "Zintegrowałem jeden z największych motoryzacyjnych systemów ERP w Europie Środkowo-Wschodniej (CEE).",
                        "Pomogłem rozbudować zespół z 3 do 11 osób, pełniąc jednocześnie rolę Scrum Mastera."
                    },
                    1
                },
                {
                    new Guid("2214aa78-3923-4498-8871-336ed00bad36"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "i4B Sp. z o.o.", "Programista", new DateOnly(2016, 6, 1), new DateOnly(2019, 9, 1),
                    new[]
                    {
                        "Zaprojektowałem i rozwinąłem regułowy system automatyzacji cen dla międzynarodowej platformy e-commerce z branży motoryzacyjnej.",
                        "Samodzielnie zaprojektowałem, rozwinąłem i utrzymywałem rozwiązanie w Sitefinity CMS obsługujące proces zwrotów klientów na rynku motoryzacyjnym Beneluksu.",
                        "Rozwinąłem system generowania szablonów e-mail obsługujący wiele typów wiadomości i kanałów sprzedaży."
                    },
                    2
                }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("82bd2baa-c2a3-4814-b63b-46e21f2f56c7"),
                new Guid("f50a9b41-da3b-4fb3-ae9c-d08162afbd0b"),
                new Guid("eb43e6c0-7693-4262-8c25-069a9d09e508"),
                new Guid("470017b4-2bea-4dcc-a8e2-7f363264b5f4"),
                new Guid("433d41dc-6a86-4d25-99ff-3a502e0a3600"),
                new Guid("2214aa78-3923-4498-8871-336ed00bad36")
            });
    }
}
