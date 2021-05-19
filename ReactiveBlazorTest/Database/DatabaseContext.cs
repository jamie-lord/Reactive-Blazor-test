using Microsoft.EntityFrameworkCore;

namespace ReactiveBlazorTest.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<PersonPto> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=ReactiveBlazorTest.db");
        }
    }
}
