using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261117135819_CreateSalesDemo : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE TABLE "SalesCategories" (
                "Id" integer PRIMARY KEY,
                "Name" text NOT NULL
            );

            CREATE TABLE "SalesProducts" (
                "Id" integer PRIMARY KEY,
                "CategoryId" integer NOT NULL REFERENCES "SalesCategories" ("Id"),
                "PriceCents" integer NOT NULL
            );

            CREATE TABLE "SalesOrders" (
                "Id" integer PRIMARY KEY,
                "ProductId" integer NOT NULL REFERENCES "SalesProducts" ("Id"),
                "Quantity" integer NOT NULL
            );

            INSERT INTO "SalesCategories" ("Id", "Name")
            SELECT id, 'Category ' || lpad(id::text, 2, '0') FROM generate_series(1, 12) AS id;

            INSERT INTO "SalesProducts" ("Id", "CategoryId", "PriceCents")
            SELECT id, 1 + id % 12, 500 + (id * 7919) % 49500 FROM generate_series(1, 5000) AS id;

            INSERT INTO "SalesOrders" ("Id", "ProductId", "Quantity")
            SELECT id, 1 + (id::bigint * 104729) % 5000, 1 + id % 9 FROM generate_series(1, 400000) AS id;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DROP TABLE "SalesOrders";
            DROP TABLE "SalesProducts";
            DROP TABLE "SalesCategories";
            """);
    }
}
