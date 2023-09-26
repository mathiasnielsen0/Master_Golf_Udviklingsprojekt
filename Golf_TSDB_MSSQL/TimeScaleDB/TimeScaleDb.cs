using Core.Interfaces;
using Core.Models;

namespace TSDB2;

public class TimeScaleDb : IDatabase
{
    public TimeScaleDb()
    {
        
    }
    
    public Task<List<HoldingsInAccount>> GetHoldings(DateTime from, DateTime to, string accountCode)
    {
        throw new NotImplementedException();
    }

    public Task<decimal> GetAvgPrices(DateTime from, DateTime to, string accountCode, int SecurityId)
    {
        throw new NotImplementedException();
    }
}