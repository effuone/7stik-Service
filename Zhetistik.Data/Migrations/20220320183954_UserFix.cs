using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zhetistik.Data.Migrations
{
    public partial class UserFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Candidates_CandidateId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CandidateId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CandidateId",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CandidateId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CandidateId",
                table: "AspNetUsers",
                column: "CandidateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Candidates_CandidateId",
                table: "AspNetUsers",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "CandidateId");
        }
    }
}
