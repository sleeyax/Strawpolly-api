using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace strawpoll.Migrations
{
    public partial class PollModels_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Polls",
                columns: table => new
                {
                    PollID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polls", x => x.PollID);
                });

            migrationBuilder.CreateTable(
                name: "PollAnswers",
                columns: table => new
                {
                    PollAnswerID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Answer = table.Column<string>(nullable: true),
                    PollID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollAnswers", x => x.PollAnswerID);
                    table.ForeignKey(
                        name: "FK_PollAnswers_Polls_PollID",
                        column: x => x.PollID,
                        principalTable: "Polls",
                        principalColumn: "PollID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollParticipants",
                columns: table => new
                {
                    PollParticipantID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PollID = table.Column<long>(nullable: false),
                    MemberID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollParticipants", x => x.PollParticipantID);
                    table.ForeignKey(
                        name: "FK_PollParticipants_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollParticipants_Polls_PollID",
                        column: x => x.PollID,
                        principalTable: "Polls",
                        principalColumn: "PollID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollVotes",
                columns: table => new
                {
                    PollVoteID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PollAnswerID = table.Column<long>(nullable: false),
                    MemberID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollVotes", x => x.PollVoteID);
                    table.ForeignKey(
                        name: "FK_PollVotes_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PollVotes_PollAnswers_PollAnswerID",
                        column: x => x.PollAnswerID,
                        principalTable: "PollAnswers",
                        principalColumn: "PollAnswerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollAnswers_PollID",
                table: "PollAnswers",
                column: "PollID");

            migrationBuilder.CreateIndex(
                name: "IX_PollParticipants_MemberID",
                table: "PollParticipants",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_PollParticipants_PollID",
                table: "PollParticipants",
                column: "PollID");

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_MemberID",
                table: "PollVotes",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_PollAnswerID",
                table: "PollVotes",
                column: "PollAnswerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollParticipants");

            migrationBuilder.DropTable(
                name: "PollVotes");

            migrationBuilder.DropTable(
                name: "PollAnswers");

            migrationBuilder.DropTable(
                name: "Polls");
        }
    }
}
