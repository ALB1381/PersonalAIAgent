using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalAIAgent.Core.Data
{
    public class ContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public Context CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            
         //for migrations
            optionsBuilder.UseSqlite("Data Source=design_time_mock.db");

            return new Context(optionsBuilder.Options);
        }
    }
}
