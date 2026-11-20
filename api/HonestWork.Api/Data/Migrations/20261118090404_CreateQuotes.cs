using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HonestWork.Api.Data.Migrations;

public partial class _20261118090404_CreateQuotes : Migration
{
        protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Quotes",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Text = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Quotes", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "Quotes",
            column: "Text",
            values: new object[]
            {
                "I'm a backend developer who decided his portfolio needed an actual backend.",
                "I lead a development team, but I still enjoy writing code myself.",
                "I play Magic: The Gathering, mostly Commander. Five decks, five different ways to cause trouble.",
                "My favorite Magic deck is led by Phage the Untouchable. I appreciate a dangerous win condition.",
                "I enjoy comics, games, technology, and conversations that suddenly become philosophical.",
                "My biggest problem with hobbies is that I keep finding new ones.",
                "The most interesting people are the ones who can teach you something unexpected.",
                "I peaked at Platinum I before Emerald was even a rank. It sounded more impressive back then.",
                "I'm a Shaco OTP. I barely have time to play anymore, but once an OTP, always an OTP.",
                "Dungeon Crawler Carl is one of my favorite series. Chaos, dark humor, and surprisingly human characters.",
                "Red Rising is one of my favorite series. Apparently I enjoy fictional worlds with terrible working conditions.",
                "If your heart beats like a drum, and your leg’s a little wet, it’s because the Reaper’s come to collect a little debt.",
                "But I am no builder. So take your time. We will wait.",
                "Pax au Telemanus!",
                "I've voluntarily paid to run through mud and climb obstacles. Apparently that's a hobby.",
                "I enjoy collecting stories about broken worlds and people stubborn enough to try fixing them.",
                "I'm a husband and a father.",
                "I have experience managing kindergarten-level chaos. Whether that's at home or at work is for you to decide.",
                "I love inline skating.",
                "My favorite gemstone is opal."
            });
    }

        protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Quotes");
    }
}
