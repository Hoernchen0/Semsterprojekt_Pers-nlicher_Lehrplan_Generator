using Microsoft.EntityFrameworkCore;

public class LernAppDbConnection : DbContext
{
    public DbSet<User> Users => Set<User>();  // Tabelle "Users"

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=lernapp.db"); // DB-Datei
    }
}
