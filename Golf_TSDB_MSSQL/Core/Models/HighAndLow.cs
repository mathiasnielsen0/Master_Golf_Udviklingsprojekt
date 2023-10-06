namespace Core.Models;

public class HighAndLow
{
    public int? SecurityId { get; set; }
    public decimal HighestPrice { get; set; }
    public decimal LowestPrice { get; set; }
}
