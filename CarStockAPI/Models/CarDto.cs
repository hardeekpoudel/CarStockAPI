public class CarDto
{
    public Guid CarId { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public int Stock { get; set; }
    public decimal? Price { get; set; }
}