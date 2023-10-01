using Core.Interfaces;
using Core.Models;
using Microsoft.Data.SqlClient;

public class MSSqlDatabase : IDatabase
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

    async Task<List<HoldingsInAccount>> IDatabase.GetHoldings(DateTime from, DateTime to, string accountCode)
    {
        var holdings = new List<HoldingsInAccount>();

        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(@"
            SELECT * 
            FROM HoldingsInAccountsT 
            WHERE NavDate BETWEEN @from AND @to 
            AND AccountCode = @accountCode", conn);

        cmd.Parameters.AddWithValue("@from", from);
        cmd.Parameters.AddWithValue("@to", to);
        cmd.Parameters.AddWithValue("@accountCode", accountCode);

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

    async Task<decimal> IDatabase.GetAvgPrices(DateTime from, DateTime to, string accountCode, int SecurityId)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand(@"
            SELECT AVG(ValuationPrice) 
            FROM HoldingsInAccountsT 
            WHERE NavDate BETWEEN @from AND @to 
            AND AccountCode = @accountCode
            AND SecurityId = @securityId", conn);

        cmd.Parameters.AddWithValue("@from", from);
        cmd.Parameters.AddWithValue("@to", to);
        cmd.Parameters.AddWithValue("@accountCode", accountCode);
        cmd.Parameters.AddWithValue("@securityId", SecurityId);

        await conn.OpenAsync();

        object result = await cmd.ExecuteScalarAsync();

        return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
    }
}
