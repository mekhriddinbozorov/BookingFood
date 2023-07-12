using BookingFood.Data;
using BookingFood.Entities;
using Microsoft.EntityFrameworkCore;
public class AppdbContext : DbContext, IAppDbContext
{
    public AppdbContext(DbContextOptions<AppdbContext> options)
        : base(options) { }
    public DbSet<User> Users { get; set; }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}