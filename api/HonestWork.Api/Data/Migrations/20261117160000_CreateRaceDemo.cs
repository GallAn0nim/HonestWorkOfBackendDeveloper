using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261117160000_CreateRaceDemo : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE TABLE "RaceComics" (
                "Id" uuid PRIMARY KEY,
                "Stock" integer NOT NULL,
                "CreatedAt" timestamptz NOT NULL DEFAULT now()
            );

            CREATE TABLE "RaceOrders" (
                "Id" bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                "ComicId" uuid NOT NULL REFERENCES "RaceComics" ("Id") ON DELETE CASCADE,
                "Customer" integer NOT NULL
            );

            CREATE INDEX "IX_RaceOrders_ComicId" ON "RaceOrders" ("ComicId");
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DROP TABLE "RaceOrders";
            DROP TABLE "RaceComics";
            """);
    }
}
