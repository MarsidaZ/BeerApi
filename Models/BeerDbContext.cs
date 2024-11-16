using BeerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BeerApi.Data;

public class BeerDbContext(DbContextOptions<BeerDbContext> options) : DbContext(options)
{
    public DbSet<Beer> Beers { get; set; }
}
