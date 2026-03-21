namespace QuantityMeasurement.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuantityMeasurement.Infrastructure.Persistence.Entities;
public interface IQuantityDbContext
{
     DbSet<History> Histories {get ; set ; }
     DbSet<User> Users { get; set; }

     int SaveChanges();
}