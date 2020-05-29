using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations
{
    public partial class edit_event_Ntitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedTitle",
                table: "Events",
                maxLength: 300,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedTitle",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Events",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }
    }
}
