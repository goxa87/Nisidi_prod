using Microsoft.EntityFrameworkCore.Migrations;

namespace EventBLib.Migrations
{
    public partial class addResizedImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MediumImage",
                table: "Events",
                maxLength: 124,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiniImage",
                table: "Events",
                maxLength: 124,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                table: "AspNetUsers",
                maxLength: 124,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MediumImage",
                table: "AspNetUsers",
                maxLength: 124,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiniImage",
                table: "AspNetUsers",
                maxLength: 124,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediumImage",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MiniImage",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "MediumImage",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MiniImage",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Photo",
                table: "AspNetUsers",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 124,
                oldNullable: true);
        }
    }
}
