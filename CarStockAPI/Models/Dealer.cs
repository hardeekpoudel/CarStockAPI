public class Dealer
{
    public Guid DealerId { get; set; }
    public string? DealerName { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime InsertDateUtc { get; set; }
    public DateTime UpdateDateUtc { get; set; }
}