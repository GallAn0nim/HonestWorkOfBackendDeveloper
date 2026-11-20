using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261119090000_ApplyCvReview : Migration
{
    private const string LinkedInUrl = "https://www.linkedin.com/in/oskar-gruszczy%C5%84ski-648aa717a/";
    private const string WebsiteUrl = "https://example.com";

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "LinkedInUrl",
            table: "CvProfiles",
            newName: "WebsiteUrl");

        migrationBuilder.UpdateData(
            table: "CvProfiles",
            keyColumn: "Id",
            keyValue: new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"),
            columns: new[] { "WebsiteUrl", "WorkPreference" },
            values: new object[] { WebsiteUrl, "Poland (CET), remote preferred" });

        migrationBuilder.UpdateData(
            table: "CvProfiles",
            keyColumn: "Id",
            keyValue: new Guid("81735658-6291-4cb7-accc-32011498c07d"),
            columns: new[] { "WebsiteUrl", "WorkPreference" },
            values: new object[] { WebsiteUrl, "Polska (CET), preferowana praca zdalna" });

        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("5ea2acf7-7870-4cf8-b4e6-2e5ebad067e2"),
            column: "Thesis",
            value: null);

        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("0b91f89c-8768-43a6-b501-90a5c3bdad27"),
            column: "Thesis",
            value: null);

        migrationBuilder.DeleteData(
            table: "CvSkills",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("a05db714-7813-4a3d-b1b2-b31743737fe7"),
                new Guid("adb6c67a-4e73-4d14-a987-02e857626f45"),
                new Guid("5aa6d180-e203-4ee6-b719-9eed50fc8cce"),
                new Guid("898caf28-c708-48f6-8208-66e499c6c356"),
                new Guid("f3c0298a-d23b-47af-962b-f53aaea8e25d"),
                new Guid("65556e26-2236-4cfa-b0aa-e19c204a5963")
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("82bd2baa-c2a3-4814-b63b-46e21f2f56c7"),
            column: "Highlights",
            value: new[]
            {
                "Architected unified product data and vehicle data platforms integrating multiple data sources across Europe.",
                "Scaled the team from 11 to 21 members (15 internal, 6 external specialists), leading their onboarding and mentoring.",
                "Serve as the primary technical contact for client stakeholders, turning business requirements into delivery plans.",
                "Led the full rollout of an order management service across Germany, handling ~20 million orders annually.",
                "Manage high-traffic 24/7 production services (up to 500 req/s) with high availability and zero-downtime deployments."
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
                "Integrated one of the largest automotive ERP systems in Central and Eastern Europe (CEE).",
                "Recruited and onboarded engineers as the team grew from 3 to 11 members, while acting as Scrum Master."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("eb43e6c0-7693-4262-8c25-069a9d09e508"),
            column: "Highlights",
            value: new[]
            {
                "Automated pricing with a rule-based system for an international automotive e-commerce platform.",
                "Single-handedly delivered a Sitefinity CMS solution for the customer returns process in the Benelux automotive market, owning client communication and two years of support.",
                "Built an email template generation system covering 16 email types, so onboarding a new sales channel took only a few CSS changes."
            });

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("470017b4-2bea-4dcc-a8e2-7f363264b5f4"),
            column: "Highlights",
            value: new[]
            {
                "Zaprojektowałem architekturę ujednoliconych platform danych produktowych i pojazdowych, integrujących wiele źródeł danych z całej Europy.",
                "Rozbudowałem zespół z 11 do 21 osób (15 wewnętrznych, 6 zewnętrznych specjalistów), prowadząc ich onboarding i mentoring.",
                "Pełnię rolę głównego kontaktu technicznego dla interesariuszy po stronie klienta, przekładając wymagania biznesowe na plany realizacji.",
                "Kierowałem pełnym wdrożeniem usługi zarządzania zamówieniami w Niemczech, obsługującej ok. 20 mln zamówień rocznie.",
                "Utrzymuję wysokoobciążone usługi produkcyjne 24/7 (do 500 req/s) z wysoką dostępnością i wdrożeniami bez przestojów."
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

        migrationBuilder.UpdateData(
            table: "CvExperiences",
            keyColumn: "Id",
            keyValue: new Guid("2214aa78-3923-4498-8871-336ed00bad36"),
            column: "Highlights",
            value: new[]
            {
                "Zautomatyzowałem ustalanie cen za pomocą regułowego systemu dla międzynarodowej platformy e-commerce z branży motoryzacyjnej.",
                "W pojedynkę dostarczyłem rozwiązanie w Sitefinity CMS obsługujące proces zwrotów klientów na rynku motoryzacyjnym Beneluksu, odpowiadając za kontakt z klientem i dwa lata utrzymania.",
                "Zbudowałem system generowania szablonów e-mail obejmujący 16 typów wiadomości, dzięki któremu uruchomienie nowego kanału sprzedaży wymagało jedynie kilku zmian w CSS."
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
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

        migrationBuilder.InsertData(
            table: "CvSkills",
            columns: new[] { "Id", "ProfileId", "Category", "Name", "SortOrder" },
            values: new object[,]
            {
                { new Guid("a05db714-7813-4a3d-b1b2-b31743737fe7"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Cloud & DevOps", "Git", 18 },
                { new Guid("adb6c67a-4e73-4d14-a987-02e857626f45"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Leadership", "Technical Leadership", 21 },
                { new Guid("5aa6d180-e203-4ee6-b719-9eed50fc8cce"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Leadership", "Team Management", 22 },
                { new Guid("898caf28-c708-48f6-8208-66e499c6c356"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Chmura i DevOps", "Git", 18 },
                { new Guid("f3c0298a-d23b-47af-962b-f53aaea8e25d"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Przywództwo", "Przywództwo techniczne", 21 },
                { new Guid("65556e26-2236-4cfa-b0aa-e19c204a5963"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Przywództwo", "Zarządzanie zespołem", 22 }
            });

        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("5ea2acf7-7870-4cf8-b4e6-2e5ebad067e2"),
            column: "Thesis",
            value: "The use of artificial intelligence algorithms to search and track player’s location based on a simple arcade game");

        migrationBuilder.UpdateData(
            table: "CvEducations",
            keyColumn: "Id",
            keyValue: new Guid("0b91f89c-8768-43a6-b501-90a5c3bdad27"),
            column: "Thesis",
            value: "Zastosowanie wybranych algorytmów sztucznej inteligencji do szukania i śledzenia gracza na przykładzie prostej gry zręcznościowej");

        migrationBuilder.UpdateData(
            table: "CvProfiles",
            keyColumn: "Id",
            keyValue: new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"),
            columns: new[] { "WebsiteUrl", "WorkPreference" },
            values: new object[] { LinkedInUrl, "Remote preferred" });

        migrationBuilder.UpdateData(
            table: "CvProfiles",
            keyColumn: "Id",
            keyValue: new Guid("81735658-6291-4cb7-accc-32011498c07d"),
            columns: new[] { "WebsiteUrl", "WorkPreference" },
            values: new object[] { LinkedInUrl, "Preferowana praca zdalna" });

        migrationBuilder.RenameColumn(
            name: "WebsiteUrl",
            table: "CvProfiles",
            newName: "LinkedInUrl");
    }
}
