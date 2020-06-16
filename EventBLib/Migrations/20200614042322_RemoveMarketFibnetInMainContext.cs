using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations
{
    public partial class RemoveMarketFibnetInMainContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketKibnets",
                columns: table => new
                {
                    MarketKibnetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    PaymentAccountBalance = table.Column<double>(nullable: false),
                    TotalMarcetCompanyCount = table.Column<int>(nullable: false),
                    MarketState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketKibnets", x => x.MarketKibnetId);
                    table.ForeignKey(
                        name: "FK_MarketKibnets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarkListCards",
                columns: table => new
                {
                    MarkListCardId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketKibnetId = table.Column<int>(nullable: false),
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
                    table.ForeignKey(
                        name: "FK_MarkListCards_MarketKibnets_MarketKibnetId",
                        column: x => x.MarketKibnetId,
                        principalTable: "MarketKibnets",
                        principalColumn: "MarketKibnetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketKibnets_UserId",
                table: "MarketKibnets",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarkListCards_MarketKibnetId",
                table: "MarkListCards",
                column: "MarketKibnetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarkListCards");

            migrationBuilder.DropTable(
                name: "MarketKibnets");
        }
    }
}
