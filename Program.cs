using BeerApi.Data;
using BeerApi.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<BeerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Minimal API endpoints
app.MapGet("/beers", BeerHandlers.GetAllBeers);
app.MapGet("/beers/{id:int}", BeerHandlers.GetBeerById)
    .WithName("GetBeerById");
app.MapPost("/beers", BeerHandlers.Create);
app.MapGet("/beers/search/{name}", BeerHandlers.SearchByName);
app.MapPut("/beers/{id:int}/rating", BeerHandlers.UpdateRatingBeer);

app.Run();
