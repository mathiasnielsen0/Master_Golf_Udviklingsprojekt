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


    public async Task<List<HoldingsInAccount>> GetHoldingsLowerThan30DayAvg(DateTime from, DateTime to, string accountCode, int SecurityId)
    {
        var holdings = new List<HoldingsInAccount>();

        using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);

        string s = @"
            WITH ValuationAverages AS (
                SELECT 
                    securityid, 
                    navdate,
                    valuationprice,
                    AVG(valuationprice) OVER (
                        PARTITION BY securityid 
                        ORDER BY navdate
                        ROWS BETWEEN 30 PRECEDING AND 1 PRECEDING
                    ) AS AverageValuationPriceLast30Days
                FROM 
                    holdings_in_accounts_t
                WHERE
                  securityid = @securityId
                  AND accountcode = @accountCode 
                  AND navdate BETWEEN @from AND @to 
            )

            SELECT 
                securityid, 
                navdate,
                valuationprice,
                averagevaluationpricelast30days
            FROM 
                ValuationAverages
            WHERE 
                valuationprice < AverageValuationPriceLast30Days
            ORDER BY 
                securityid, 
                navdate;
            ";


        using NpgsqlCommand cmd = new NpgsqlCommand(s, conn);

        cmd.Parameters.AddWithValue("@from", from);
        cmd.Parameters.AddWithValue("@to", to);
        cmd.Parameters.AddWithValue("@accountCode", accountCode);
        cmd.Parameters.AddWithValue("@securityId", SecurityId);


        await conn.OpenAsync();

        using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var securityId = reader["securityid"] == DBNull.Value ? -1 : Convert.ToInt32(reader["securityid"] ?? -1);
            holdings.Add(new HoldingsInAccount
            {
                NavDate = Convert.ToDateTime(reader["navdate"]),
                SecurityId = securityId,
                ValuationPrice = reader["valuationprice"] as decimal?
            });
        }

        return holdings;
    }

    public async Task<List<HighAndLow>> GetHighestAndLowestPrices(DateTime from, DateTime to, string accountCode)
    {
        var holdings = new List<HighAndLow>();

        using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);

        string s = @"
            SELECT securityid, MAX(valuationprice) as MaxValuationPrice, MIN(valuationprice) as MinValuationPrice
            FROM holdings_in_accounts_t
            WHERE navdate BETWEEN @from AND @to 
              AND accountcode = @accountCode
            GROUP BY securityid;
            ";


        using NpgsqlCommand cmd = new NpgsqlCommand(s, conn);

        cmd.Parameters.AddWithValue("@from", from);
        cmd.Parameters.AddWithValue("@to", to);
        cmd.Parameters.AddWithValue("@accountCode", accountCode);


        await conn.OpenAsync();

        using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var securityId = reader["SecurityId"] == DBNull.Value ? -1 : Convert.ToInt32(reader["SecurityId"] ?? -1);
            try
            {
                holdings.Add(new HighAndLow
                {
                    LowestPrice = Convert.ToDecimal(reader["MinValuationPrice"]),
                    HighestPrice = Convert.ToDecimal(reader["MaxValuationPrice"]),
                    SecurityId = securityId
                });
            }
            catch { }
        }

        return holdings;
    }
}