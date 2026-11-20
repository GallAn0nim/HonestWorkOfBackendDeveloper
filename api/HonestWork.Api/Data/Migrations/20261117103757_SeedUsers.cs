using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261117103757_SeedUsers : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Users",
            columns: new[] { "Id", "Username", "Email", "PasswordHash" },
            values: new object[,]
            {
                { new Guid("27e1fafe-8f69-4e03-a2bc-0f44fd895ac8"), "Admin", "admin@HonestWorkOfBackendDeveloper.pl", "AQAAAAIAAYagAAAAEG9MXNdHr49xLyaxVgh02ImdsILvP+q2+/qCNeZkWoy39Lix7q2oyKoMAYxhwcYnXQ==" },
                { new Guid("6123828f-3f83-45ec-8ccb-0ab7a91de005"), "Test", "test@HonestWorkOfBackendDeveloper.pl", "AQAAAAIAAYagAAAAEB17d/YGMsfWlmEmZhEwBcP/OAunRbLQXyx4PXLFBqW+dTWWIH8byJ7h/j6CmUmA/Q==" },
                { new Guid("3f07ae6d-8520-4e9a-a09d-f8ad3f0b7af0"), "Oskar", "oskar@HonestWorkOfBackendDeveloper.pl", "AQAAAAIAAYagAAAAELf/7saItsB3Wyk1NQCLn5cAyY2Ex7khNFBF7LCA7YGBVwMXuwdYuh9EkkeMcAgwlQ==" },
                { new Guid("57030f65-acd2-452c-b000-4d7d658d5301"), "Administrator", "administrator@HonestWorkOfBackendDeveloper.pl", "AQAAAAIAAYagAAAAEPimgzxYTZuQuLJoUA1fYCAgyV2kBpNPI8ie6qzHYcVQlwsnC1ppy4yBK+iJsRSRoQ==" },
                { new Guid("900c4296-f010-4f8c-9643-40393f24d933"), "HonestBackendDeveloper", "HonestBackendDeveloper@HonestWorkOfBackendDeveloper.pl", "AQAAAAIAAYagAAAAEAPFHmIlCP7ljs8LprFoFFPwFxJm6InWUNIZLYEe8A6imf7EdupULaIQUCOV5p7PzA==" }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Users",
            keyColumn: "Id",
            keyValues: new object[]
            {
                new Guid("27e1fafe-8f69-4e03-a2bc-0f44fd895ac8"),
                new Guid("6123828f-3f83-45ec-8ccb-0ab7a91de005"),
                new Guid("3f07ae6d-8520-4e9a-a09d-f8ad3f0b7af0"),
                new Guid("57030f65-acd2-452c-b000-4d7d658d5301"),
                new Guid("900c4296-f010-4f8c-9643-40393f24d933")
            });
    }
}
