namespace BeerApi.Service;

public record BeerResponse(int Id, string Name, string Type, double AverageRating);

public record CreateBeerRequest(string Name, string Type, int? Rating);


