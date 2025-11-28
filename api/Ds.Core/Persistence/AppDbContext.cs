using Bogus;
using Ds.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ds.Core.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerKey> CustomerKeys { get; set; }
    public DbSet<TradeRecommendation> TradeRecommendations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Randomizer.Seed = new Random(123);
        var faker = new Faker();

        var customer = new Customer
        {
            Id = 1,
            Email = faker.Internet.Email()
        };

        modelBuilder.Entity<Customer>().HasData(customer);

        base.OnModelCreating(modelBuilder);
    }
}