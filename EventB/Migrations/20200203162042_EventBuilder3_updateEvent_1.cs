using Microsoft.EntityFrameworkCore.Migrations;

namespace EventB.Migrations
{
    public partial class EventBuilder3_updateEvent_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WillGo",
                table: "Events",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "WillGo",
                table: "Events");
        }
    }
}
