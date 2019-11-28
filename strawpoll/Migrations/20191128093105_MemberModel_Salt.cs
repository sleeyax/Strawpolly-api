using Microsoft.EntityFrameworkCore.Migrations;

namespace strawpoll.Migrations
{
    public partial class MemberModel_Salt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "Members",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "Members");
        }
    }
}
