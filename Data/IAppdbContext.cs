using BookingFood.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFood.Data;
public interface IAppDbContext
{
    DbSet<User> Users {get;set;}
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}