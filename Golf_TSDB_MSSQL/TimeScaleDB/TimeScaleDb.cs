using Core.Interfaces;
using Core.Models;

namespace TSDB2;

public class TimeScaleDb : IDatabase
{
    public TimeScaleDb()
    {
        
    }

    public Task<PriceQueryModel> GetPrices(DateTime fra, DateTime til, string productCode)
    {
        throw new NotImplementedException();
    }
}