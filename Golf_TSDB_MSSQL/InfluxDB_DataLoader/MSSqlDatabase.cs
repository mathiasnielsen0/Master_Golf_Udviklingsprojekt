using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;

public class MSSqlDatabase
{
    private readonly string _connectionString;

    public MSSqlDatabase()
    {
        if (Environment.MachineName == "B32XL0ELTOD466S") // Allans PC
            _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Holdings;Integrated Security=True;MultipleActiveResultSets=True;Encrypt=false;TrustServerCertificate=true";
        else
            _connectionString = @"Data Source=.;Initial Catalog=AHO;Integrated Security=True;MultipleActiveResultSets=True;Encrypt=false;TrustServerCertificate=true";
    }

    public MSSqlDatabase(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<List<HoldingsInAccount>> GetHoldings(DateTime date)
    {
        var holdings = new List<HoldingsInAccount>();

        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(@"
            SELECT * 
            FROM HoldingsInAccountsT 
            WHERE NavDate = @date", conn);

        cmd.Parameters.AddWithValue("@date", date);

        await conn.OpenAsync();

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var securityId = reader["SecurityId"] == DBNull.Value ? -1 : Convert.ToInt32(reader["SecurityId"] ?? -1);
            holdings.Add(new HoldingsInAccount
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

        return holdings;
    }

}
