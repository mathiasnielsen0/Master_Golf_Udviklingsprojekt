using Core.Models;

namespace Core.Interfaces;

public interface IDatabase
{
    Task<PriceQueryModel> GetPriceAverage(DateTime fra, DateTime til, string productCode);
    
    /// <summary>
    /// Get "amount" of HoldingsInAccount, ordered ascending by date
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    Task<PriceQueryModel> GetXFirst(int amount);
}