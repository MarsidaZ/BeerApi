using BeerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BeerApi.Data;

public class BeerDbContext(DbContextOptions<BeerDbContext> options) : DbContext(options)
{
    public DbSet<Beer> Beers { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
        modelBuilder.Entity<Beer>()
            .HasMany(b => b.Ratings)      
            .WithOne(r => r.Beer)         
            .HasForeignKey(r => r.BeerId); 

    }
}