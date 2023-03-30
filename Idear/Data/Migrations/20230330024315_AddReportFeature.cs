using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Idear.Data.Migrations
{
    public partial class AddReportFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reactes_AspNetUsers_UserId",
                table: "Reactes");

            migrationBuilder.DropForeignKey(
                name: "FK_Reactes_Ideas_IdeaId",
                table: "Reactes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reactes",
                table: "Reactes");

            migrationBuilder.RenameTable(
                name: "Reactes",
                newName: "React");

            migrationBuilder.RenameIndex(
                name: "IX_Reactes_UserId",
                table: "React",
                newName: "IX_React_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reactes_IdeaId",
                table: "React",
                newName: "IX_React_IdeaId");

            migrationBuilder.AddColumn<DateTime>(
                name: "BannedDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_React",
                table: "React",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_React_AspNetUsers_UserId",
                table: "React",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_React_Ideas_IdeaId",
                table: "React",
                column: "IdeaId",
                principalTable: "Ideas",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_React_AspNetUsers_UserId",
                table: "React");

            migrationBuilder.DropForeignKey(
                name: "FK_React_Ideas_IdeaId",
                table: "React");

            migrationBuilder.DropPrimaryKey(
                name: "PK_React",
                table: "React");

            migrationBuilder.DropColumn(
                name: "BannedDate",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "React",
                newName: "Reactes");

            migrationBuilder.RenameIndex(
                name: "IX_React_UserId",
                table: "Reactes",
                newName: "IX_Reactes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_React_IdeaId",
                table: "Reactes",
                newName: "IX_Reactes_IdeaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reactes",
                table: "Reactes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reactes_AspNetUsers_UserId",
                table: "Reactes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reactes_Ideas_IdeaId",
                table: "Reactes",
                column: "IdeaId",
                principalTable: "Ideas",
                principalColumn: "Id");
        }
    }
}
