using Core.Models;

namespace Core.Interfaces;

public interface IDatabase
{
    Task<PriceQueryModel> GetHolding(DateTime fra, DateTime til, string productCode);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fra"></param>
    /// <param name="til"></param>
    /// <param name="productCode"></param>
    /// <returns></returns>
    Task<PriceQueryModel> GetPrices(DateTime fra, DateTime til, string productCode);
}