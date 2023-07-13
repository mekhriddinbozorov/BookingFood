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
        modelBuilder.Entity<User>()
            .HasKey(e => e.Id);
            
        modelBuilder.Entity<User>()
            .Property(e => e.Fullname)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(e => e.Username)
            .HasMaxLength(50);

        modelBuilder.Entity<User>()
            .Property(e => e.Phone)
            .HasMaxLength(20);

        modelBuilder.Entity<User>()
            .Property(e => e.CreatedAt)
            .HasDefaultValue(DateTime.UtcNow);

        modelBuilder.Entity<User>()
            .Property(e => e.ModifiedAt)
            .HasDefaultValue(DateTime.UtcNow);
        base.OnModelCreating(modelBuilder);
    }
}