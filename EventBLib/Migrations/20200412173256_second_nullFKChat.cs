using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations
{
    public partial class second_nullFKChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Events_EventId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_EventId",
                table: "Chats");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "Chats",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_EventId",
                table: "Chats",
                column: "EventId",
                unique: true,
                filter: "[EventId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Events_EventId",
                table: "Chats",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Events_EventId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_EventId",
                table: "Chats");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "Chats",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_EventId",
                table: "Chats",
                column: "EventId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Events_EventId",
                table: "Chats",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
