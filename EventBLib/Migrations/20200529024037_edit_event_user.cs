using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations
{
    public partial class edit_event_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgeRestrictions",
                table: "Events",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Events",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlockedUser",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedCity",
                table: "AspNetUsers",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeRestrictions",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "IsBlockedUser",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NormalizedCity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "AspNetUsers");
        }
    }
}
