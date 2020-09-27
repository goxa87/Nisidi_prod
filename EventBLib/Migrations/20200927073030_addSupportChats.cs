using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations
{
    public partial class addSupportChats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupportChatId",
                table: "SupportMessages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SupportChats",
                columns: table => new
                {
                    SupportChatId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportChats", x => x.SupportChatId);
                    table.ForeignKey(
                        name: "FK_SupportChats_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportMessages_SupportChatId",
                table: "SupportMessages",
                column: "SupportChatId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportChats_UserId",
                table: "SupportChats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportMessages_SupportChats_SupportChatId",
                table: "SupportMessages",
                column: "SupportChatId",
                principalTable: "SupportChats",
                principalColumn: "SupportChatId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportMessages_SupportChats_SupportChatId",
                table: "SupportMessages");

            migrationBuilder.DropTable(
                name: "SupportChats");

            migrationBuilder.DropIndex(
                name: "IX_SupportMessages_SupportChatId",
                table: "SupportMessages");

            migrationBuilder.DropColumn(
                name: "SupportChatId",
                table: "SupportMessages");
        }
    }
}
