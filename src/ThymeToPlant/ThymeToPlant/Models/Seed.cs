#nullable enable

namespace ThymeToPlant.Models;

public class Seed
{
    public Guid Id { get; set; }
    public string CommonName { get; set; } = string.Empty;
    public string? Variety { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? Barcode { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public int? QuantityRemaining { get; set; }
    public string? PhotoPath { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
