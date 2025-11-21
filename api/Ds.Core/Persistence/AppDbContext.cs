using Ds.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ds.Core.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerKey> CustomerKeys { get; set; }
    public DbSet<TradeRecommendation> TradeRecommendations { get; set; }
}