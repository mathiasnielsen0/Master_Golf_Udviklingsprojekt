using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Globalization;
using Core.Interfaces;

public class TestRunner
{
    private readonly IDatabase _dbToTest;
    private readonly string[] accountCodes;
    private readonly int[] securityIds;
    private readonly Stopwatch _sw;

    public TestRunner(IDatabase dbToTest) 
    {
        _dbToTest = dbToTest;
        _sw = new Stopwatch();

        // The 30 most frequent SecurityIds
        securityIds = new[]
            {
                1000600, 1002163, 1002185, 1002037, //1001032, 1007182, 1002243, 1007173,
                //1001289, 1002521, 1007556, 1001277, 1001681, 1005940, 1003365, 1005267,
                //1003364, 1003362, 1007369, 1003292, 1008544, 1002483, 1001924, 1001425,
                //1003015, 1000941, 1001021, 1009221, 1003587, 1007682
            };

        // The 31 most frequent AccountCodes
        accountCodes = new[]
            {
                "1119", "1117", "1118", "1103", "1125", "1290", "1038", "1063", "1047",
                //"1156", "1036", "1066", "1062", "1046", "1045", "1340", "1037", "1039",
                //"1041", "1044", "1071", "1072", "1109", "1126", "1040", "1108", "1107",
                //"1065", "1131", "1058", "1001"
            };
    }

    /// <summary>
    /// Returns number of milliseconds ellapsed
    /// </summary>
    /// <returns></returns>
    public async Task<long> RunTests()
    {
        var results = new List<string>();
        results.Add($"Start;End;AccountCode;; Time ms");
        var resultsAvg = new List<string>();
        resultsAvg.Add($"Start;End;AccountCode;SecurityId; Time ms");

        DateTime startDate = DateTime.ParseExact("2012-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime endDate = DateTime.ParseExact("2012-11-30", "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var monthsToRunPerLoop = 3;
        var totalRuns = Math.Ceiling((decimal)accountCodes.Count() * (((endDate.Year - startDate.Year) * 12) + monthsToRunPerLoop + endDate.Month - startDate.Month) / monthsToRunPerLoop) - 4;
        int runCounter = 0;

        while (startDate <= endDate)
        {
            DateTime to = startDate.AddMonths(monthsToRunPerLoop).AddDays(-1);

            foreach (var accountCode in accountCodes)
            {
                var startEllapsed = _sw.ElapsedMilliseconds;
                _sw.Start();
                await _dbToTest.GetHoldings( startDate, to, accountCode);
                _sw.Stop();
                
                //Console.WriteLine($"Run GetHoldings with {startDate.ToShortDateString()} - {to.ToShortDateString()}, Acc. {accountCode}. Ellapsed {_sw.ElapsedMilliseconds}ms");

                foreach (var securityId in securityIds)
                {
                    _sw.Start();
                    await _dbToTest.GetAvgPrices(startDate, to, accountCode, securityId);
                    _sw.Stop();
                    
                    //Console.WriteLine($"Run GetAvgPrices with {startDate.ToShortDateString()} - {to.ToShortDateString()}, Acc. {accountCode}, secId. {securityId}. Ellapsed {_sw.ElapsedMilliseconds}ms");
                }

                Console.WriteLine($"{_dbToTest.GetType().FullName} : Run {runCounter++} / {totalRuns}. Ellapsed: {_sw.ElapsedMilliseconds - startEllapsed}ms");
            }
            
            startDate = startDate.AddMonths(monthsToRunPerLoop).AddDays(-1);
        }

        Console.WriteLine($"Run all tests for {_dbToTest.GetType().FullName}, time: {_sw.ElapsedMilliseconds}ms");
        var ellapsed = _sw.ElapsedMilliseconds;
        _sw.Reset();

        return ellapsed;
    }
}
