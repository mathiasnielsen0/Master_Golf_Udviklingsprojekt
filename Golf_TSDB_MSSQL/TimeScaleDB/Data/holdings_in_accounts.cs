using Microsoft.EntityFrameworkCore;

namespace TSDB2.Data;

[Keyless]
public class holdings_in_accounts_t
{
    public string? accountcode { get; set; }	
    public DateTime navdate { get; set; }	
    public string? localcurrencycode { get; set; }	
    public double? marketvalue { get; set; }	
    public string? numberofshare { get; set; }	
    public string? securityid { get; set; }	
    public string? securityname { get; set; }	
    public string? name { get; set; }	
    public string? holdingtype { get; set; }	
    public double? percentage { get; set; }
}