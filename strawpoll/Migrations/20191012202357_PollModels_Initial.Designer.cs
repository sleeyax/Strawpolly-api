﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using strawpoll.Models;

namespace strawpoll.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20191012202357_PollModels_Initial")]
    partial class PollModels_Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("strawpoll.Models.Member", b =>
                {
                    b.Property<long>("MemberID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("Password");

                    b.HasKey("MemberID");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("strawpoll.Models.Poll", b =>
                {
                    b.Property<long>("PollID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("PollID");

                    b.ToTable("Polls");
                });

            modelBuilder.Entity("strawpoll.Models.PollAnswer", b =>
                {
                    b.Property<long>("PollAnswerID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Answer");

                    b.Property<long>("PollID");

                    b.HasKey("PollAnswerID");

                    b.HasIndex("PollID");

                    b.ToTable("PollAnswers");
                });

            modelBuilder.Entity("strawpoll.Models.PollParticipant", b =>
                {
                    b.Property<long>("PollParticipantID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("MemberID");

                    b.Property<long>("PollID");

                    b.HasKey("PollParticipantID");

                    b.HasIndex("MemberID");

                    b.HasIndex("PollID");

                    b.ToTable("PollParticipants");
                });

            modelBuilder.Entity("strawpoll.Models.PollVote", b =>
                {
                    b.Property<long>("PollVoteID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("MemberID");

                    b.Property<long>("PollAnswerID");

                    b.HasKey("PollVoteID");

                    b.HasIndex("MemberID");

                    b.HasIndex("PollAnswerID");

                    b.ToTable("PollVotes");
                });

            modelBuilder.Entity("strawpoll.Models.PollAnswer", b =>
                {
                    b.HasOne("strawpoll.Models.Poll", "Poll")
                        .WithMany("Answers")
                        .HasForeignKey("PollID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("strawpoll.Models.PollParticipant", b =>
                {
                    b.HasOne("strawpoll.Models.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("strawpoll.Models.Poll", "Poll")
                        .WithMany("Participants")
                        .HasForeignKey("PollID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("strawpoll.Models.PollVote", b =>
                {
                    b.HasOne("strawpoll.Models.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("strawpoll.Models.PollAnswer", "Answer")
                        .WithMany("Votes")
                        .HasForeignKey("PollAnswerID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
