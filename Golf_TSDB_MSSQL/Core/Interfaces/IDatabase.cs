using Core.Models;

namespace Core.Interfaces;

public interface IDatabase
{
    public Task<List<HoldingsInAccount>> GetHoldings(DateTime from, DateTime to, string accountCode);


    public Task<decimal> GetAvgPrices(DateTime from, DateTime to, string accountCode, int SecurityId);

    public Task<List<HoldingsInAccount>> GetHoldingsLowerThan30DayAvg(DateTime from, DateTime to, string accountCode, int SecurityId);

}