using Microsoft.EntityFrameworkCore;

namespace strawpoll.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollAnswer> PollAnswers { get; set; }
        public DbSet<PollParticipant> PollParticipants { get; set; }
        public DbSet<PollVote> PollVotes { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Member>().ToTable("Members");
            builder.Entity<Poll>().ToTable("Polls");
            builder.Entity<PollAnswer>().ToTable("PollAnswers");
            builder.Entity<PollParticipant>().ToTable("PollParticipants");
            builder.Entity<PollVote>().ToTable("PollVotes");
        }

    }
}