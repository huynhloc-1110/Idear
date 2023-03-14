using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Idear.Data.Migrations
{
    public partial class AddAnonymousToIdeasComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "Ideas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "Comments");
        }
    }
}
