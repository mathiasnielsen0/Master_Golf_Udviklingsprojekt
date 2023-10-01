using InfluxDB;
using TSDB2;

Console.WriteLine("Running testrunner CLI");
var timeScaletestrunner = new TestRunner(new TimeScaleDb());
var msSqltestRunner = new TestRunner(new MSSqlDatabase());
//var influxDbRunner = new TestRunner(new InfluxDBRepository());

List<long> msSqltestRunnerTimings = new List<long>();
List<long> timeScaletestrunnerTimings = new List<long>();
//List<long> influxDbRunnerTimings = new List<long>();

var numOfRuns = 10;
for (int i = 0; i < numOfRuns; i++)
{
    msSqltestRunnerTimings.Add(await msSqltestRunner.RunTests());    
    timeScaletestrunnerTimings.Add(await timeScaletestrunner.RunTests());
    //influxDbRunnerTimings.Add(await influxDbRunner.RunTests());    
}

var fileStrings = new List<string>(numOfRuns + 10);
fileStrings.Add($"MsSql;TimescaleDb;InfluxDb");
for (int i = 0; i < numOfRuns; i++)
    //fileStrings.Add($"{msSqltestRunnerTimings[i]};{timeScaletestrunnerTimings[i]};{influxDbRunnerTimings[i]}");
    fileStrings.Add($"{msSqltestRunnerTimings[i]};{timeScaletestrunnerTimings[i]};");

Console.WriteLine("Writing to files");
File.WriteAllLines($"Testrun_{DateTime.Now.ToFileTime()}.csv", fileStrings.Select(x => x.ToString()).ToList());