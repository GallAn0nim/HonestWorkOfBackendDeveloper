using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261117114649_CreateCv : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CvProfiles",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                LanguageCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                LinkedInUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CvProfiles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CvEducations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                Institution = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Degree = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Field = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Thesis = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                SortOrder = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CvEducations", x => x.Id);
                table.ForeignKey(
                    name: "FK_CvEducations_CvProfiles_ProfileId",
                    column: x => x.ProfileId,
                    principalTable: "CvProfiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CvExperiences",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                Company = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                Role = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                Highlights = table.Column<string[]>(type: "text[]", nullable: false),
                SortOrder = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CvExperiences", x => x.Id);
                table.ForeignKey(
                    name: "FK_CvExperiences_CvProfiles_ProfileId",
                    column: x => x.ProfileId,
                    principalTable: "CvProfiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CvLanguages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                SortOrder = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CvLanguages", x => x.Id);
                table.ForeignKey(
                    name: "FK_CvLanguages_CvProfiles_ProfileId",
                    column: x => x.ProfileId,
                    principalTable: "CvProfiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CvSkills",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                SortOrder = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CvSkills", x => x.Id);
                table.ForeignKey(
                    name: "FK_CvSkills_CvProfiles_ProfileId",
                    column: x => x.ProfileId,
                    principalTable: "CvProfiles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CvEducations_ProfileId",
            table: "CvEducations",
            column: "ProfileId");

        migrationBuilder.CreateIndex(
            name: "IX_CvExperiences_ProfileId",
            table: "CvExperiences",
            column: "ProfileId");

        migrationBuilder.CreateIndex(
            name: "IX_CvLanguages_ProfileId",
            table: "CvLanguages",
            column: "ProfileId");

        migrationBuilder.CreateIndex(
            name: "IX_CvProfiles_LanguageCode",
            table: "CvProfiles",
            column: "LanguageCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CvSkills_ProfileId",
            table: "CvSkills",
            column: "ProfileId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CvEducations");

        migrationBuilder.DropTable(
            name: "CvExperiences");

        migrationBuilder.DropTable(
            name: "CvLanguages");

        migrationBuilder.DropTable(
            name: "CvSkills");

        migrationBuilder.DropTable(
            name: "CvProfiles");
    }
}
