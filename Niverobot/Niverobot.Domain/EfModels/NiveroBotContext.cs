using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Niverobot.Domain.EfModels
{
    public class NiveroBotContext : DbContext
    {
        public NiveroBotContext(DbContextOptions<NiveroBotContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        public DbSet<Reminder> Reminders { get; set; }



    }
}
