using System.Threading.Channels;
using InfluxDB;
using TSDB2;

Console.WriteLine("Running testrunner CLI");

Console.WriteLine("Use MSSQLDB? Enter y/n");
var useMsSql = Console.ReadLine() == "y";
Console.WriteLine("Use TimeScaleDb? Enter y/n");
var useTimeScaleDb = Console.ReadLine() == "y";
Console.WriteLine("Use InfluxDBRepository? Enter y/n");
var useInfluxDb = Console.ReadLine() == "y";

Console.WriteLine("How many runs? Approx. 30 sec each run pr. database");
var numOfRuns = int.Parse(Console.ReadLine());


var timeScaletestrunner = new TestRunner(new TimeScaleDb());
var msSqltestRunner = new TestRunner(new MSSqlDatabase());
var influxDbRunner = new TestRunner(new InfluxDBRepository());

List<long> msSqltestRunnerTimings = new List<long>();
List<long> timeScaletestrunnerTimings = new List<long>();
List<long> influxDbRunnerTimings = new List<long>();

for (int i = 0; i < numOfRuns; i++)
{
    Console.WriteLine($"RUNNING CHOSEN DATABASES: {i}/{numOfRuns}");
    if (useMsSql) msSqltestRunnerTimings.Add(await msSqltestRunner.RunTests());    
    if (useTimeScaleDb) timeScaletestrunnerTimings.Add(await timeScaletestrunner.RunTests());
    if (useInfluxDb) influxDbRunnerTimings.Add(await influxDbRunner.RunTests());    
}

var fileStrings = new List<string>(numOfRuns + 10);
fileStrings.Add($"MsSql;TimescaleDb;InfluxDb;");
for (int i = 0; i < numOfRuns; i++)
{
    var str = "";
    if (useMsSql) str += msSqltestRunnerTimings[i] + ";";
    if (useTimeScaleDb) str += timeScaletestrunnerTimings[i] + ";";
    if (useInfluxDb) str += influxDbRunnerTimings[i] + ";";
    
    fileStrings.Add(str);
}

Console.WriteLine("Writing to files");
File.WriteAllLines($"Testrun_{DateTime.Now.ToFileTime()}.csv", fileStrings.Select(x => x.ToString()).ToList());