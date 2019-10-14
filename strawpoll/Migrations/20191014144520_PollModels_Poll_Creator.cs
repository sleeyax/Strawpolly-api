using Microsoft.EntityFrameworkCore.Migrations;

namespace strawpoll.Migrations
{
    public partial class PollModels_Poll_Creator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MemberID",
                table: "Polls",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Polls_MemberID",
                table: "Polls",
                column: "MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_Polls_Members_MemberID",
                table: "Polls",
                column: "MemberID",
                principalTable: "Members",
                principalColumn: "MemberID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Polls_Members_MemberID",
                table: "Polls");

            migrationBuilder.DropIndex(
                name: "IX_Polls_MemberID",
                table: "Polls");

            migrationBuilder.DropColumn(
                name: "MemberID",
                table: "Polls");
        }
    }
}
