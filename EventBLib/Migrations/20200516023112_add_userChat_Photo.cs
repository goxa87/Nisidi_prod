using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations
{
    public partial class add_userChat_Photo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatPhoto",
                table: "UserChats",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatPhoto",
                table: "UserChats");
        }
    }
}
