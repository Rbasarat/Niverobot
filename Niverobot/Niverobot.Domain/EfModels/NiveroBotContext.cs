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
        
        public DbSet<Reminder> Reminders { get; set; }



    }
}
