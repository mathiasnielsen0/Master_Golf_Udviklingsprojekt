using InfluxDB.Client.Writes;
using InfluxDB.Client;
using System;
using System.Threading.Tasks;
using InfluxDB.Client.Api.Domain;
using Newtonsoft.Json.Linq;
using Core.Models;
using InfluxDB.Client.Core.Flux.Domain;

namespace InfluxDB;

public class InfluxDBRepository : IInfluxDBRepository
{
    private readonly InfluxDBClient _client;

    public InfluxDBRepository()
    {
        // Insert API Token here:
        _client = new InfluxDBClient("http://localhost:8086", "2VXaEmksYuvUgTsQWjThcVIpLZ52UYnzDNJnuS1F8gtD1zuJ9a5uwCT4W4SCWGITz0cZgw5cQ7FqVPBYojd7IQ==");
    }

    public InfluxDBRepository(string url, string token)
    {
        char[] Token = token.ToCharArray();
        _client = new InfluxDBClient(url, token);
    }

    public void WriteDataBatchAsync(string bucket, string org, List<Core.Models.HoldingsInAccount> records)
    {
        try
        {
            using var writeApi = _client.GetWriteApi();
            var points = records.Select(record => CreatePointData(record)).ToList();
            writeApi.WritePoints(points, bucket, org);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public void WriteDataAsync(string bucket, string org, HoldingsInAccount holdingInAccount)
    {
        WriteDataAsync(bucket, org, CreatePointData(holdingInAccount));
    }

    private PointData CreatePointData(Core.Models.HoldingsInAccount holdingInAccount)
    {
        return PointData.Measurement("holdings_in_account") // Define measurement name
            .Tag("AccountCode", holdingInAccount.AccountCode) // Add tags
            .Tag("Name", holdingInAccount.Name)
            .Field("LocalCurrencyCode", holdingInAccount.LocalCurrencyCode)
            .Field("BondType", holdingInAccount.BondType)
            .Field("HoldingType", holdingInAccount.HoldingType)
            .Field("MarketValue", holdingInAccount.MarketValue.HasValue ? Convert.ToDouble(holdingInAccount.MarketValue.Value) : default(double?)) // Add fields
            .Field("NumberOfShare", holdingInAccount.NumberOfShare.HasValue ? Convert.ToDouble(holdingInAccount.NumberOfShare.Value) : default(double?))
            .Field("Percentage", holdingInAccount.Percentage.HasValue ? Convert.ToDouble(holdingInAccount.Percentage.Value) : default(double?))
            .Timestamp(holdingInAccount.NavDate.ToUniversalTime(), WritePrecision.S); // Set timestamp
    }

    public void WriteDataAsync(string bucket, string org, PointData point)
    {
        try
        {
            using var writeApi = _client.GetWriteApi();
            writeApi.WritePoint(point, bucket, org);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void WriteDataAsync(string bucket, string org, List<PointData> points)
    {
        using var writeApi = _client.GetWriteApi();

        foreach (var item in points)
        {
            writeApi.WritePoint(item, bucket, org);
        }
        //writeApi.WritePoints(points, bucket, org);
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

    public async Task<List<FluxRecord>> QueryDataMultipleRecordsAsync(string bucket, string org, string query)
    {
        var tables = await _client.GetQueryApi().QueryAsync(query, org);

        // For this example, return first value (adjust this to your needs)
        if (tables != null && tables.Count > 0 && tables[0].Records != null && tables[0].Records.Count > 0)
        {
            return tables[0].Records;
        }

        return new List<FluxRecord>();
    }

    public async Task<FluxRecord> QueryDataOneRecordAsync(string bucket, string org, string query)
    {
        var tables = await _client.GetQueryApi().QueryAsync(query, org);

        // For this example, return first value (adjust this to your needs)
        if (tables != null && tables.Count > 0 && tables[0].Records != null && tables[0].Records.Count > 0)
        {
            return tables[0].Records[0];
        }

        return null;
    }

    public async Task<List<HoldingsInAccount>> QueryDataAsync(string bucket, string org, string accountCode, DateTime navDate)
    {
        var startDate = navDate.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");
        var endDate = navDate.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ssZ");

        var flux = $@"from(bucket:""{bucket}"")
          |> range(start: {startDate}, stop: {endDate})
          |> filter(fn: (r) => r._measurement == ""holdings_in_account"" and r.AccountCode == ""{accountCode}"")";

        var tables = await _client.GetQueryApi().QueryAsync(flux, org);

        var resultList = new List<HoldingsInAccount>();

        if (tables != null && tables.Count > 0)
        {
            foreach (var record in tables[0].Records)
            {
                foreach (var item in record.Values)
                {
                    Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");  // Print every field's key and value
                }

                var holding = new HoldingsInAccount
                {
                    AccountCode = record.GetValueByKey("AccountCode")?.ToString(),
                    NavDate = (DateTime)(record.GetTime()?.ToDateTimeUtc()),
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

    public async Task<int> GetRowCountAsync(string bucket, string org)
    {
        var flux = $@"from(bucket:""{bucket}"")
                  |> range(start: 0)
                  |> filter(fn: (r) => r._measurement == ""holdings_in_account"")
                  |> count(column: ""_value"")";

        var tables = await _client.GetQueryApi().QueryAsync(flux, org);

        if (tables != null && tables.Count > 0 && tables[0].Records != null && tables[0].Records.Count > 0)
        {
            // Sum op alle tællinger for at få den samlede antal rækker.
            return tables[0].Records.Sum(record => Convert.ToInt32(record.GetValue()));
        }

        return 0;
    }


}
