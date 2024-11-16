using BeerApi.Data;
using BeerApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeerApi.Service;

public static class BeerHandlers
{
    public static async Task<Ok<List<Beer>>> GetAllBeers([FromServices] BeerDbContext db)
    {
        var beers = await db.Beers.ToListAsync();
        return TypedResults.Ok(beers);
    }

    public static async Task<Results<Ok<Beer>, NotFound>> GetBeerById([FromServices] BeerDbContext db, [FromRoute] int id)
    {
        var beer = await db.Beers.FindAsync(id);
        return beer != null ? TypedResults.Ok(beer) : TypedResults.NotFound();
    }

    public static async Task<Results<CreatedAtRoute<Beer>, BadRequest<string>>> Create([FromServices] BeerDbContext db, [FromBody] Beer beer)
    {
        var existingBeer = await db.Beers.FirstOrDefaultAsync(b => b.Name == beer.Name);
        if (existingBeer != null)
            return TypedResults.BadRequest("A beer with this name already exists.");

        if (beer.Ratings.Any(r => r < 1 || r > 5))
            return TypedResults.BadRequest("All ratings must be between 1 and 5.");

        db.Beers.Add(beer);
        await db.SaveChangesAsync();

        return TypedResults.CreatedAtRoute(
                routeName: "GetBeerById",
                routeValues: new { id = beer.Id },
                value: beer);
    }

    public static async Task<Results<Ok<List<Beer>>, NotFound<string>>> SearchByName([FromServices] BeerDbContext db, [FromRoute] string name)
    {
        var beers = await db.Beers
            .Where(b => EF.Functions.ILike(b.Name, $"%{name}%"))
            .ToListAsync();

        return beers.Any()
            ? TypedResults.Ok(beers)
            : TypedResults.NotFound("Beer with this name does not exist");
    }
   
    public static async Task<Results<NoContent, NotFound, BadRequest<string>>> UpdateRatingBeer([FromServices] BeerDbContext db, [FromRoute] int id, [FromQuery] int rating)
    {
        var beer = await db.Beers.FindAsync(id);
        if (beer == null)
            return TypedResults.NotFound();

        if (rating < 1 || rating > 5)
            return TypedResults.BadRequest("Rating must be between 1 and 5.");

        beer.Ratings.Add(rating);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
