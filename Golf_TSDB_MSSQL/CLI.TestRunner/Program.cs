using CLI.TestRunner;
using Core.Interfaces;
using InfluxDB;
using TSDB2;

List<IDatabase> dbsToTest = new List<IDatabase>(); 

Console.WriteLine("Running testrunner CLI");

Console.WriteLine("Use MSSQLDB? Enter y/n");
if (Console.ReadLine() == "y") dbsToTest.Add(new MSSqlDatabase()); 
Console.WriteLine("Use TimeScaleDb? Enter y/n");
if (Console.ReadLine() == "y") dbsToTest.Add(new TimeScaleDb()); 
Console.WriteLine("Use InfluxDBRepository? Enter y/n");
if (Console.ReadLine() == "y") dbsToTest.Add(new InfluxDBRepository()); 

Console.WriteLine("How many runs? Approx. 30 sec each run pr. database");
var numOfRuns = int.Parse(Console.ReadLine());

var results = new List<ResultModel>();

for (int i = 0; i < numOfRuns; i++)
{
    Console.WriteLine($"RUNNING CHOSEN DATABASES: {i+1}/{numOfRuns}");
    foreach (var db in dbsToTest)
        results.Add(await new TestRunner(db).RunTests());
}

PrintFiles(results);

void PrintFiles(List<ResultModel> results)
{
    var dbResultsGroupings = results.GroupBy(x => x.DbType).ToList();
    
    foreach (var dbResults in dbResultsGroupings)
    {
        var csvLines = new List<string>();
        csvLines.Add(dbResults.Key);
        csvLines.Add("GetHoldings;GetAvgValuationPrice;GetLessThan30DAvg;GetHighAndLow");
        var dbRes = dbResults.ToList();

        foreach (var r in dbRes)
            csvLines.Add($"{r.HoldingsMs};{r.AvgMs};{r.LessThan30DAvgMs};{r.HighLowMs}");

        File.WriteAllLines($"Testrun_{dbResults.Key}_{DateTime.Now.ToFileTime()}.csv", csvLines);
    }
    
    Console.WriteLine("Writing to files");
}
