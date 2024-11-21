using BeerApi.Data;
using BeerApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeerApi.Service;

public static class BeerHandlers
{
    public static async Task<Ok<List<BeerResponse>>> GetAllBeers([FromServices] BeerDbContext db)
    {

        var beersWithAverageRating = await db.Beers
          .Include(b => b.Ratings)
          .Select(b => new BeerResponse(
              b.Id,
              b.Name,
              b.Type,
              b.Ratings.Count > 0 ? b.Ratings.Average(r => r.Value) : 0
          ))
          .ToListAsync();

        return TypedResults.Ok(beersWithAverageRating);
    }

    public static async Task<Results<Ok<BeerResponse>, NotFound>> GetBeerById([FromServices] BeerDbContext db, [FromRoute] int id)
    {
        var beer = await db.Beers
           .Include(b => b.Ratings)
           .Where(b => b.Id == id)
           .Select(b => new BeerResponse(
               b.Id,
               b.Name,
               b.Type,
               b.Ratings.Count > 0 ? b.Ratings.Average(r => r.Value) : 0
           ))
           .FirstOrDefaultAsync();

        return beer != null ? TypedResults.Ok(beer) : TypedResults.NotFound();
    }

    public static async Task<Results<CreatedAtRoute<BeerResponse>, BadRequest<string>>> Create([FromServices] BeerDbContext db, [FromBody] CreateBeerRequest beerRequest)
    {
        var existingBeer = await db.Beers.FirstOrDefaultAsync(b => b.Name == beerRequest.Name);
        if (existingBeer != null)
            return TypedResults.BadRequest("A beer with this name already exists.");

        if (beerRequest.Rating < 1 || beerRequest.Rating > 5)
            return TypedResults.BadRequest("All ratings must be between 1 and 5.");

        var beer = new Beer
        {
            Name = beerRequest.Name,
            Type = beerRequest.Type,
            Ratings = []
        };

        if (beerRequest.Rating.HasValue)
        {
            beer.Ratings.Add(new Rating
            {
                Value = beerRequest.Rating.Value
            });
        }

        db.Beers.Add(beer);
        await db.SaveChangesAsync();

        var beerResponse = new BeerResponse(beer.Id, beer.Name, beer.Type, beer.Ratings.Count != 0 ? beer.Ratings.Average(r => r.Value) : 0 );

        return TypedResults.CreatedAtRoute(
            routeName: "GetBeerById",
            routeValues: new { id = beer.Id },
            value: beerResponse);
    }

    public static async Task<Results<Ok<List<BeerResponse>>, NotFound<string>>> SearchByName([FromServices] BeerDbContext db, [FromRoute] string name)
    {
        var beers = await db.Beers
             .Where(b => EF.Functions.ILike(b.Name, $"%{name}%"))
             .Select(b => new BeerResponse(
                 b.Id,
                 b.Name,
                 b.Type,
                 b.Ratings.Count > 0 ? b.Ratings.Average(r => r.Value) : 0
             ))
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

        beer.Ratings = [];
        beer.Ratings.Add(new Rating { BeerId = id, Value = rating });
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
