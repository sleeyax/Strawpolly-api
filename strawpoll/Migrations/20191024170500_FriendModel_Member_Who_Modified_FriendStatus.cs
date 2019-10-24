using Microsoft.EntityFrameworkCore.Migrations;

namespace strawpoll.Migrations
{
    public partial class FriendModel_Member_Who_Modified_FriendStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MemberWhoModifiedID",
                table: "Friends",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friends_MemberWhoModifiedID",
                table: "Friends",
                column: "MemberWhoModifiedID");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_Members_MemberWhoModifiedID",
                table: "Friends",
                column: "MemberWhoModifiedID",
                principalTable: "Members",
                principalColumn: "MemberID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_Members_MemberWhoModifiedID",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_MemberWhoModifiedID",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "MemberWhoModifiedID",
                table: "Friends");
        }
    }
}
