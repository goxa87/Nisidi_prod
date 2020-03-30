using Microsoft.EntityFrameworkCore.Migrations;

namespace EventB.Migrations
{
    public partial class _2903_addFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PersonFriendId",
                table: "Friends",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonFriendId",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "AspNetUsers");
        }
    }
}
