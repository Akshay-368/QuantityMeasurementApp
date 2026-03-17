using Microsoft.EntityFrameworkCore;
using QuantityMeasurement.Infrastructure.Persistence.Entities;

namespace QuantityMeasurement.Infrastructure.Persistence;

public class QuantityDbContext : DbContext
{
    public QuantityDbContext(DbContextOptions<QuantityDbContext> options)
        : base(options)
    {
    }

    public DbSet<History> Histories { get; set; }
}