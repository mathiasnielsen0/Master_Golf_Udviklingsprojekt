using Core.Models;
using InfluxDB;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;
using MSSQL;
using System.Diagnostics;
using System.Drawing;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await WriteOneRecordToInfluxDBAndReadItAgain();
        //await WriteManyRecordsToInfluxDBAndReadSomeAgain();
        //await SeedInfluxDBFromSqlDatabase();
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

    private static void WriteOneHoldingRecordsToInfluxDBAndReadSomeAgain()
    {
        Console.WriteLine("Save Holding records to InfluxDB...");
        IInfluxDBRepository InfluxRepo = new InfluxDBRepository();

        // Create a DataPoint for Influx with some testdata
        string bucket = "Holdings";
        string org = "Sparinvest";
        string measu = "holdings_in_account";

    }

    private static async Task WriteManyRecordsToInfluxDBAndReadSomeAgain()
    {
        Console.WriteLine("Save records to InfluxDB...");
        IInfluxDBRepository InfluxRepo = new InfluxDBRepository();

        // Create a DataPoint for Influx with some testdata
        string bucket = "testBucket";
        string org = "Sparinvest";
        string measu = "testMeasurement";

        var dataPoints = new List<PointData>();
        var dato = new DateTime(2023, 9, 12, 3, 3, 0);

        for (int i = 0; i < 1000; i++)
        {
            var point = PointData
                .Measurement(measu)
                .Timestamp(dato.ToUniversalTime(), WritePrecision.S)
                .Tag("accountCode", (40000 + i).ToString())
                .Field("value", 120.45 + i);

            dato = dato.AddSeconds(1);
            dataPoints.Add(point);
        }

        // Save testdata to Influx
        InfluxRepo.WriteDataAsync(bucket, org, dataPoints);


        // Load some records and verify correct data from Influx 
        Console.WriteLine("Load records and verify from InfluxDB...");


        // Adjusting the query to potentially retrieve more records for the given measurement
        string query = $"from(bucket: \"{bucket}\") |> range(start: -1h) |> filter(fn: (r) => r._measurement == \"{measu}\")";
        List<FluxRecord> records = await InfluxRepo.QueryDataMultipleRecordsAsync(bucket, org, query); // Assuming you'll implement QueryDataMultipleRecordsAsync

        bool allRecordsMatch = true;
        foreach (var record in records)
        {
            // Assuming the accountCode can be fetched using GetValueByKey method
            int accountCode = int.Parse(record.GetValueByKey("accountCode").ToString());

            if (record.GetValueByKey("_value").ToString() != (123.45 + accountCode).ToString())
            {
                allRecordsMatch = false;
                break;
            }
        }

        if (allRecordsMatch)
        {
            Console.WriteLine("Data verified: All records match.");
        }
        else
        {
            Console.WriteLine("Mismatch: Some records don't match.");
        }
    }

    private static async Task WriteOneRecordToInfluxDBAndReadItAgain()
    {
        Console.WriteLine("Save a record to InfluxDB...");
        IInfluxDBRepository InfluxRepo = new InfluxDBRepository();

        // Create a DataPoint for Influx with some testdata
        string bucket = "testBucket";
        string org = "Sparinvest";
        string measu = "testMeasurement";

        var point = PointData
            .Measurement(measu)
            .Timestamp(DateTime.UtcNow, WritePrecision.S)
            .Tag("accountCode", "1089")
            .Field("value", 124.45);

        // Save testdata to Influx
        InfluxRepo.WriteDataAsync(bucket, org, point);

        // Load the record again from Influx 
        Console.WriteLine("Load record from InfluxDB...");
        string query = $"from(bucket: \"{bucket}\") |> range(start: -1h) |> filter(fn: (r) => r._measurement == \"{measu}\")";
        FluxRecord record = await InfluxRepo.QueryDataOneRecordAsync(bucket, org, query);

        // Compare data to verify that it is written and read correct.
        if (record != null && record.GetValueByKey("_value").ToString() == "123.45") // Assuming FluxRecord has a method GetField
        {
            Console.WriteLine("Data verified: The record written matches the record read.");
        }
        else
        {
            Console.WriteLine("Mismatch: The record written doesn't match the record read.");
        }
    }


    private static async Task SeedInfluxDBFromSqlDatabase()
    {
        Console.WriteLine("Starting seeding data to InfluxDB...");
        IInfluxDBRepository InfluxRepo = new InfluxDBRepository();

        IMyDbContext dbContext = new MyDbContext();

        var firstDate = new DateTime(2012, 1, 1); // dbContext.HoldingsInAccounts.Min(i => i.NavDate);
        var lastDate = new DateTime(2015, 1, 1); // = dbContext.HoldingsInAccounts.Max(i => i.NavDate);

        Console.WriteLine($"Running from {firstDate:dd/MM/yyyy} to {lastDate:dd/MM/yyyy}");

        // run from firstDate to lastDate

        for (DateTime date = firstDate; date <= lastDate; date = date.AddDays(1))
        {
            Console.Write($"\rLoading {date:dd/MM/yyyy}...      ");
            List<Core.Models.HoldingsInAccount> recordsForDate = dbContext.HoldingsInAccounts.Where(i => i.NavDate == date).Select(i => new Core.Models.HoldingsInAccount
            {
                AccountCode = i.AccountCode,
                BondType = i.BondType,
                HoldingType = i.HoldingType,
                LocalCurrencyCode = i.LocalCurrencyCode,
                MarketValue = i.MarketValue,
                Name = i.Name,
                NavDate = i.NavDate,
                NumberOfShare = i.NumberOfShare,
                Percentage = i.Percentage
            }).ToList();

            var sw = new Stopwatch();
            sw.Start();

            int iCount = 0;
            var recordsForDateCount = recordsForDate.Count;
            int batchCount = 0;
            if (recordsForDateCount > 0)
            {
                const int batchSize = 2000; // Adjust this based on your requirements and observations.
                for (int i = 0; i < recordsForDateCount; i += batchSize)
                {
                    var batch = recordsForDate.Skip(i).Take(batchSize).ToList();
                    iCount += batch.Count;
                    Console.Write($"\r{date:dd/MM/yyyy}:   Writing batch {++batchCount}   {iCount} / {recordsForDateCount}      ");
                    InfluxRepo.WriteDataBatchAsync("Holdings", "Sparinvest", batch);
                }
                var ellapsed = sw.ElapsedMilliseconds;

                long influxCount = await InfluxRepo.GetRowCountAsync("Holdings", "Sparinvest");
                Console.WriteLine($"{(ellapsed / iCount):N1} ms/row - InfluxDB count: {influxCount}");
            }
            //int iCount = recordsForDate.Count;
            //int i = 0;
            //foreach (var item in recordsForDate)
            //{
            //    Console.Write($"\r{date:dd/MM/yyyy}:   {++i} / {iCount}      ");
            //    InfluxRepo.WriteDataAsync("Holdings", "Sparinvest", item);
            //}
            //Console.WriteLine();
        }
    }
}