namespace Core.Models;

public class PriceQueryModel
{
    public string Productcode { get; set; }
    public string Currencycode { get; set; }
    public string ProductName { get; set; }
    public decimal NAV { get; set; }
    public decimal TNA { get; set; }
    public DateTime NavDate { get; set; }
    public DateTime CalculationTimeStamp { get; set; }
    
    //Evt. Join data herunder
}