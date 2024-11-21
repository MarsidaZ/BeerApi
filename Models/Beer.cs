namespace BeerApi.Models;

public class Beer
{
    public int Id { get; private set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public ICollection<Rating> Ratings { get; set; }
}
