using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations.Marketing
{
    public partial class add_marketing1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarkListCards",
                columns: table => new
                {
                    MarkListCardId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    ImageLink = table.Column<string>(nullable: true),
                    AHref = table.Column<string>(nullable: true),
                    IsPayed = table.Column<bool>(nullable: false),
                    PaymentAccount = table.Column<double>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    PayedDate = table.Column<DateTime>(nullable: false),
                    PublicedSince = table.Column<DateTime>(nullable: false),
                    PublicedDue = table.Column<DateTime>(nullable: false),
                    ShawnQuantity = table.Column<int>(nullable: false),
                    TirnTrigger = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkListCards", x => x.MarkListCardId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarkListCards");
        }
    }
}
