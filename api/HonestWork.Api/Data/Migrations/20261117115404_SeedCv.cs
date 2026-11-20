using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261117115404_SeedCv : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "CvProfiles",
            columns: new[] { "Id", "LanguageCode", "FullName", "Email", "LinkedInUrl" },
            values: new object[,]
            {
                { new Guid("81735658-6291-4cb7-accc-32011498c07d"), "pl", "Oskar Gruszczyński", "oskar171@gmail.com", "https://www.linkedin.com/in/oskar-gruszczy%C5%84ski-648aa717a/" },
                { new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "en", "Oskar Gruszczyński", "oskar171@gmail.com", "https://www.linkedin.com/in/oskar-gruszczy%C5%84ski-648aa717a/" }
            });

        migrationBuilder.InsertData(
            table: "CvEducations",
            columns: new[] { "Id", "ProfileId", "Institution", "Degree", "Field", "Thesis", "StartDate", "EndDate", "SortOrder" },
            values: new object[,]
            {
                { new Guid("0b91f89c-8768-43a6-b501-90a5c3bdad27"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Politechnika Koszalińska", "Inżynier", "Programowanie komputerów i sieci informatycznych", "Zastosowanie wybranych algorytmów sztucznej inteligencji do szukania i śledzenia gracza na przykładzie prostej gry zręcznościowej", new DateOnly(2013, 1, 1), new DateOnly(2017, 1, 1), 0 },
                { new Guid("91db7e75-5186-4987-b192-b7e572fcbfae"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Zespół Szkół Ponadgimnazjalnych w Białogardzie", "Technikum informatyczne", null, null, new DateOnly(2009, 9, 1), new DateOnly(2013, 4, 26), 1 },
                { new Guid("5ea2acf7-7870-4cf8-b4e6-2e5ebad067e2"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Koszalin University of Technology", "Engineer", "Computer Programming and Information Networks", "Application of selected artificial intelligence algorithms to finding and tracking a player in a simple arcade game", new DateOnly(2013, 1, 1), new DateOnly(2017, 1, 1), 0 },
                { new Guid("8b414631-504a-40a8-a9c2-a5e1378a1ded"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Zespół Szkół Ponadgimnazjalnych in Białogard", "Technical school, computer science", null, null, new DateOnly(2009, 9, 1), new DateOnly(2013, 4, 26), 1 }
            });

        migrationBuilder.InsertData(
            table: "CvLanguages",
            columns: new[] { "Id", "ProfileId", "Name", "Level", "SortOrder" },
            values: new object[,]
            {
                { new Guid("33fb0139-46b7-4836-aebc-3a780196e892"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Polski", "Ojczysty", 0 },
                { new Guid("7d08bec8-8ccb-4f91-a24a-56c88b36c991"), new Guid("81735658-6291-4cb7-accc-32011498c07d"), "Angielski", "Średniozaawansowany (B1)", 1 },
                { new Guid("87020e2f-6528-46e2-8370-a5541e4eb91a"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "Polish", "Native", 0 },
                { new Guid("c2faec80-854e-40f3-8893-8ef033b9b266"), new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec"), "English", "Intermediate (B1)", 1 }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "CvProfiles",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("81735658-6291-4cb7-accc-32011498c07d"),
                new Guid("7b675253-494a-47a9-b7c0-5feea17f6fec")
            });
    }
}
