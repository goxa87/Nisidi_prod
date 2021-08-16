using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations
{
    public partial class remove_read_message : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Read",
                table: "Messages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Read",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
