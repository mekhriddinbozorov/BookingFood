using Microsoft.EntityFrameworkCore;

public class AppdbContext : DbContext
{
    public AppdbContext(DbContextOptions<AppdbContext> options)
        :base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}