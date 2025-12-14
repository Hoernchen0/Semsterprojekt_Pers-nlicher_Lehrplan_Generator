using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace LernApp.Data
{
    // Design-time factory for EF Core tools (migrations)
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LernAppDbContext>
    {
        public LernAppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<LernAppDbContext>();

            // Use a local file for design-time migrations to avoid platform-specific paths
            var migrationsDb = "Data Source=./lernapp_migrations.db";
            builder.UseSqlite(migrationsDb);

            return new LernAppDbContext(builder.Options);
        }
    }
}
