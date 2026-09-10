using Microsoft.EntityFrameworkCore;

namespace PersonalAIAgent.Core.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            
        }

        public DbSet<API> APIs { get; set; }
    }
}
