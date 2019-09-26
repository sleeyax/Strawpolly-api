using Microsoft.EntityFrameworkCore;

namespace strawpoll.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder) => builder.Entity<Member>().ToTable("Members");
    }
}