using Microsoft.EntityFrameworkCore.Internal;
using strawpoll.Models;

namespace strawpoll
{
    public class DbInitializer
    {
        /// <summary>
        /// Initializes the database and adds dummy data
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="reset">Set to true if you want to drop (delete) the whole database first</param>
        public static void Initialize(DatabaseContext context, bool reset = false)
        {
            // NOTE: use https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/ if you don't want to lose the data in the database
            if (reset) context.Database.EnsureDeleted();

            context.Database.EnsureCreated();

            if (context.Members.Any()) return;
            
            context.Members.AddRange(
                new Member {email = "jeff@gmail.com", firstName = "Jeff", lastName = "Vermeulen"}, 
                new Member {email = "kevin.vlaey@hotmail.comw", firstName = "Kevin", lastName = "Vlaeyemans"}
                
                );
            context.SaveChanges();
        }
    }
}