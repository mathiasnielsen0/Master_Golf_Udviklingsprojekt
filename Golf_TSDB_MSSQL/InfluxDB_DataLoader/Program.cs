using InfluxDB;
using MSSQL;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting seeding data to InfluxDB...");
        IInfluxDBRepository InfluxRepo = new InfluxDBRepository();

        IMyDbContext dbContext = new MyDbContext();

        var firstDate = dbContext.HoldingsInAccounts.Min(i => i.NavDate);
        var lastDate = dbContext.HoldingsInAccounts.Max(i => i.NavDate);

        Console.WriteLine($"Running from {firstDate:dd/MM/yyyy} to {lastDate:dd/MM/yyyy}");

        // run from firstDate to lastDate
        
        for (DateTime date = firstDate; date <= lastDate; date = date.AddDays(1))
        {
            Console.Write($"\rLoading {date:dd/MM/yyyy}...      ");
            List<Core.Models.HoldingsInAccount> recordsForDate = dbContext.HoldingsInAccounts.Where(i => i.NavDate == date).Select(i => new Core.Models.HoldingsInAccount {
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
                const int batchSize = 5000; // Adjust this based on your requirements and observations.
                for (int i = 0; i < recordsForDateCount; i += batchSize)
                {
                    var batch = recordsForDate.Skip(i).Take(batchSize).ToList();
                    iCount += batch.Count;
                    Console.Write($"\r{date:dd/MM/yyyy}:   Writing batch {++batchCount}   {iCount} / {recordsForDateCount}      ");
                    InfluxRepo.WriteDataBatchAsync("Holdings", "Sparinvest", batch);
                }
                var ellapsed = sw.ElapsedMilliseconds;

                Console.WriteLine($"{(ellapsed / iCount):N1} ms/row");
            }
            //int iCount = l.Count;
            //int i = 0;
            //foreach (var item in l)
            //{
            //    Console.Write($"\r{date:dd/MM/yyyy}:   {++i} / {iCount}      ");
            //    InfluxRepo.WriteDataAsync("Holdings", "Sparinvest", item);
            //}
            //Console.WriteLine();
        }


    }
}