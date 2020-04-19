using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations
{
    public partial class change_friend_to_FriendUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentUserId",
                table: "Friends");

            migrationBuilder.AddColumn<string>(
                name: "FriendUserId",
                table: "Friends",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FriendUserId",
                table: "Friends");

            migrationBuilder.AddColumn<string>(
                name: "CurrentUserId",
                table: "Friends",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
