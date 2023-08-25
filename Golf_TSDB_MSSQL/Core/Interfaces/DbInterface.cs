using Core.Models;

namespace Core.Interfaces;

public interface IDatabase
{
    Task<List<PriceQueryModel>> GetPrices(DateTime fra, DateTime til, string productCode);
}