namespace Core.Models;

// HoldingsInAccounts
public class HoldingsInAccount
{
    public string AccountCode { get; set; } // AccountCode (length: 20)
    public DateTime NavDate { get; set; } // NavDate
    public string LocalCurrencyCode { get; set; } // LocalCurrencyCode (length: 3)
    public decimal? MarketValue { get; set; } // MarketValue
    public decimal? NumberOfShare { get; set; } // NumberOfShare
    public int? SecurityId { get; set; }
    public string SecurityName { get; set; }
    public string BondType { get; set; } // BondType (length: 250)
    public string HoldingType { get; set; } // HoldingType (length: 50)
    public decimal? Percentage { get; set; } // Percentage
    public decimal? ValuationPrice { get; set; }
}
