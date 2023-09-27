using System.Diagnostics;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using TSDB2.Data;

namespace TSDB2;

public class TimeScaleDb : IDatabase
{
    private readonly TimeScaleDbContext _dbContext;

    public TimeScaleDb(TimeScaleDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<HoldingsInAccount>> GetHoldings(DateTime from, DateTime to, string accountCode)
    {
        var queryString = $"SELECT * FROM holdings_in_accounts_t WHERE navdate > '{from.ToString("yyyy-MM-dd")}' AND navdate < '{to.ToString("yyyy-MM-dd")}' AND accountcode = '{accountCode}'";

        var holdings = _dbContext.holdings_in_accounts_t.FromSqlRaw(queryString).ToList();

        return holdings.Select(x => new HoldingsInAccount()
        {
            AccountCode = x.accountcode,
            NavDate = x.navdate,
            LocalCurrencyCode = x.localcurrencycode,
            MarketValue = (decimal?)x.marketvalue,
            NumberOfShare = decimal.Parse(x.numberofshare), 
            SecurityId = x.securityid,
            SecurityName = x.securityname,
            BondType = x.bondtype,
            HoldingType = x.holdingtype,
            Percentage = (decimal?)x.percentage,
            ValuationPrice = (decimal?)x.valuationprice,
        }).ToList();
    }

    public async Task<decimal> GetAvgPrices(DateTime from, DateTime to, string accountCode, int SecurityId)
    {
        var queryString = $"SELECT AVG(valuationprice) FROM holdings_in_accounts_t WHERE navdate > '{from.ToString("yyyy-MM-dd")}' AND navdate < '{to.ToString("yyyy-MM-dd")}' AND accountcode = '{accountCode}' AND securityid = '{SecurityId}'";
        var holdings = _dbContext.holdings_in_accounts_t.FromSqlRaw(queryString).Single();

        return 5;
    }
}