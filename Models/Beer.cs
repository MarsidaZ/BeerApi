namespace BeerApi.Models;

public class Beer
{
    public int Id { get; private set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public List<int> Ratings { get; set; } = [];
    public double AverageRating => Ratings.Count != 0 ? Ratings.Average() : 0;
}
