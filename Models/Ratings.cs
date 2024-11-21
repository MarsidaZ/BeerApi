namespace BeerApi.Models;

public class Rating
{
    public int Id { get; set; }           
    public int BeerId { get; set; }     
    public int Value { get; set; }        

    public  Beer Beer { get; set; }
}