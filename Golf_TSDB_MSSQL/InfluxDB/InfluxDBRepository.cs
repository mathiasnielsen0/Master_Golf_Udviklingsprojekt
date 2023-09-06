using InfluxDB.Client.Writes;
using InfluxDB.Client;
using System;
using System.Threading.Tasks;
using InfluxDB.Client.Api.Domain;
using Newtonsoft.Json.Linq;
using Core.Models;

namespace InfluxDB;

public class InfluxDBRepository : IInfluxDBRepository
{
    private readonly InfluxDBClient _client;

    public InfluxDBRepository()
    {
        _client = new InfluxDBClient("http://localhost:8086", "0bc2793bb567d000");
    }

    public InfluxDBRepository(string url, string token)
    {
        char[] Token = token.ToCharArray();
        _client = new InfluxDBClient(url, token);
    }

    public void WriteDataBatchAsync(string bucket, string org, List<Core.Models.HoldingsInAccount> records)
    {
        using var writeApi = _client.GetWriteApi();
        var points = records.Select(record => CreatePointData(record)).ToList();
        writeApi.WritePoints(points, bucket, org);
    }

    public void WriteDataAsync(string bucket, string org, HoldingsInAccount holdingInAccount)
    {
        WriteDataAsync(bucket, org, CreatePointData(holdingInAccount));
    }

    private PointData CreatePointData(Core.Models.HoldingsInAccount holdingInAccount)
    {
        return PointData.Measurement("holdings_in_account") // Define measurement name
            .Tag("AccountCode", holdingInAccount.AccountCode) // Add tags
            .Field("LocalCurrencyCode", holdingInAccount.LocalCurrencyCode)
            .Field("Name", holdingInAccount.Name)
            .Field("BondType", holdingInAccount.BondType)
            .Field("HoldingType", holdingInAccount.HoldingType)
            .Field("MarketValue", holdingInAccount.MarketValue.HasValue ? Convert.ToDouble(holdingInAccount.MarketValue.Value) : default(double?)) // Add fields
            .Field("NumberOfShare", holdingInAccount.NumberOfShare.HasValue ? Convert.ToDouble(holdingInAccount.NumberOfShare.Value) : default(double?))
            .Field("Percentage", holdingInAccount.Percentage.HasValue ? Convert.ToDouble(holdingInAccount.Percentage.Value) : default(double?))
            .Timestamp(holdingInAccount.NavDate, WritePrecision.Ns); // Set timestamp
    }

    public void WriteDataAsync(string bucket, string org, PointData point)
    {
        using var writeApi = _client.GetWriteApi();
        writeApi.WritePoint(point, bucket, org);
    }

    public async Task<string> QueryDataAsync(string bucket, string org, string query)
    {
        var flux = $"from(bucket:\"{bucket}\") |> range(start: -1h) |> filter(fn: (r) => r._measurement == \"{query}\")";
        var tables = await _client.GetQueryApi().QueryAsync(flux, org);

        // For this example, return first value (adjust this to your needs)
        if (tables != null && tables.Count > 0 && tables[0].Records != null && tables[0].Records.Count > 0)
        {
            return tables[0].Records[0].GetValue().ToString();
        }

        return null;
    }

    public async Task<List<HoldingsInAccount>> QueryDataAsync(string bucket, string org, string accountCode, DateTime navDate)
    {
        var flux = $@"from(bucket:""{bucket}"")
              |> range(start: {navDate.AddHours(-1):O}, stop: {navDate.AddHours(1):O})
              |> filter(fn: (r) => r._measurement == ""holdings_in_account"" and r.AccountCode == ""{accountCode}"")";

        var tables = await _client.GetQueryApi().QueryAsync(flux, org);

        var resultList = new List<HoldingsInAccount>();

        if (tables != null && tables.Count > 0)
        {
            foreach (var record in tables[0].Records)
            {
                var holding = new HoldingsInAccount
                {
                    AccountCode = record.GetValueByKey("AccountCode")?.ToString(),
                    NavDate = Convert.ToDateTime(record.GetTime()), // Assuming NavDate is the timestamp
                    LocalCurrencyCode = record.GetValueByKey("LocalCurrencyCode")?.ToString(),
                    MarketValue = Convert.ToDecimal(record.GetValueByKey("MarketValue")),
                    NumberOfShare = Convert.ToDecimal(record.GetValueByKey("NumberOfShare")),
                    Name = record.GetValueByKey("Name")?.ToString(),
                    BondType = record.GetValueByKey("BondType")?.ToString(),
                    HoldingType = record.GetValueByKey("HoldingType")?.ToString(),
                    Percentage = Convert.ToDecimal(record.GetValueByKey("Percentage"))
                };

                resultList.Add(holding);
            }
        }

        return resultList;
    }

    public async Task<List<HoldingsInAccount>> QueryDataAsync(string bucket, string org, string accountCode, DateTime fromNavDate, DateTime toNavDate)
    {
        var flux = $@"from(bucket:""{bucket}"")
              |> range(start: {fromNavDate.AddHours(-1):O}, stop: {toNavDate.AddHours(1):O})
              |> filter(fn: (r) => r._measurement == ""holdings_in_account"" and r.AccountCode == ""{accountCode}"")";

        var tables = await _client.GetQueryApi().QueryAsync(flux, org);

        var resultList = new List<HoldingsInAccount>();

        if (tables != null && tables.Count > 0)
        {
            foreach (var record in tables[0].Records)
            {
                var holding = new HoldingsInAccount
                {
                    AccountCode = record.GetValueByKey("AccountCode")?.ToString(),
                    NavDate = Convert.ToDateTime(record.GetTime()), // Assuming NavDate is the timestamp
                    LocalCurrencyCode = record.GetValueByKey("LocalCurrencyCode")?.ToString(),
                    MarketValue = Convert.ToDecimal(record.GetValueByKey("MarketValue")),
                    NumberOfShare = Convert.ToDecimal(record.GetValueByKey("NumberOfShare")),
                    Name = record.GetValueByKey("Name")?.ToString(),
                    BondType = record.GetValueByKey("BondType")?.ToString(),
                    HoldingType = record.GetValueByKey("HoldingType")?.ToString(),
                    Percentage = Convert.ToDecimal(record.GetValueByKey("Percentage"))
                };

                resultList.Add(holding);
            }
        }

        return resultList;
    }

}
