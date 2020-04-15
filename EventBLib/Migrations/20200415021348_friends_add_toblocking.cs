using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations
{
    public partial class friends_add_toblocking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonFriendId",
                table: "Friends");

            migrationBuilder.AddColumn<string>(
                name: "CurrentUserId",
                table: "Friends",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Friends",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AnonMessages",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentUserId",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "AnonMessages",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "PersonFriendId",
                table: "Friends",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
