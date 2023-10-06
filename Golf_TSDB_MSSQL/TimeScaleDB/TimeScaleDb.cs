using System.Diagnostics;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql;
using TSDB2.Data;

namespace TSDB2;

public class TimeScaleDb : IDatabase
{
    private const string _connectionString = "Server=localhost;User Id=postgres;Password=1234;Database=example;";
    public TimeScaleDb()
    {
    }
    
    public async Task<List<HoldingsInAccount>> GetHoldings(DateTime from, DateTime to, string accountCode)
    {
        using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        
        cmd.CommandText = $"SELECT * FROM holdings_in_accounts_t WHERE navdate > '{from.ToString("yyyy-MM-dd")}' AND navdate < '{to.ToString("yyyy-MM-dd")}' AND accountcode = '{accountCode}'";
        await conn.OpenAsync();
        NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        var result = new List<HoldingsInAccount>();
        while (await reader.ReadAsync())
        {
            var securityId = reader["SecurityId"] == DBNull.Value ? -1 : Convert.ToInt32(reader["SecurityId"] ?? -1);
            result.Add(new HoldingsInAccount()
            {
                AccountCode = reader["AccountCode"].ToString(),
                NavDate = Convert.ToDateTime(reader["NavDate"]),
                LocalCurrencyCode = reader["LocalCurrencyCode"].ToString(),
                MarketValue = reader["MarketValue"] as decimal?,
                NumberOfShare = reader["NumberOfShare"] as decimal?,
                SecurityId = securityId,
                SecurityName = reader["SecurityName"]?.ToString() ?? "",
                BondType = reader["BondType"].ToString(),
                HoldingType = reader["HoldingType"].ToString(),
                Percentage = reader["Percentage"] as decimal?,
                ValuationPrice = reader["ValuationPrice"] as decimal?
            });
        }
        
        return result;
    }

    public async Task<decimal> GetAvgPrices(DateTime from, DateTime to, string accountCode, int SecurityId)
    {
        using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);

        using var cmd = new NpgsqlCommand(@"
            SELECT AVG(valuationprice) 
            FROM holdings_in_accounts_t 
            WHERE navdate BETWEEN @from AND @to 
            AND accountcode = @accountCode
            AND securityid = @securityId", conn);

        cmd.Parameters.AddWithValue("@from", from);
        cmd.Parameters.AddWithValue("@to", to);
        cmd.Parameters.AddWithValue("@accountCode", accountCode);
        cmd.Parameters.AddWithValue("@securityId", SecurityId);

        await conn.OpenAsync();
        object? result = await cmd.ExecuteScalarAsync();

        return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
    }

    public Task<List<HoldingsInAccount>> GetHoldingsLowerThan30DayAvg(DateTime from, DateTime to, string accountCode, int SecurityId)
    {
        throw new NotImplementedException();
    }

    public Task<List<HighAndLow>> GetHighestAndLowestPrices(DateTime from, DateTime to, string accountCode)
    {
        throw new NotImplementedException();
    }
}