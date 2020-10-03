using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations
{
    public partial class addBlockingInUserChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlockedInChat",
                table: "UserChats",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlockedInChat",
                table: "UserChats");
        }
    }
}
