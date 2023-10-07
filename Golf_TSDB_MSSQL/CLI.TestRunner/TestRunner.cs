using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Globalization;
using CLI.TestRunner;
using Core.Interfaces;

public class TestRunner
{
    private readonly IDatabase _dbToTest;
    private readonly string[] accountCodes;
    private readonly int[] securityIds;
    private readonly Stopwatch _sw;
    private readonly DateTime start;
    private readonly DateTime end;

    public TestRunner(IDatabase dbToTest)
    {
        _dbToTest = dbToTest;
        _sw = new Stopwatch();
        start = new DateTime(2012, 1, 1);
        end = new DateTime(2012, 6, 1);

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
    public async Task<ResultModel> RunTests()
    {
        var rm = new ResultModel()
        {
            DbType = _dbToTest.GetType().FullName,
        };

        for (int i = 0; i < accountCodes.Length; i++)
        {
            _sw.Restart();
            await _dbToTest.GetHoldings(start, end, accountCodes[i]);
            rm.HoldingsMs += _sw.ElapsedMilliseconds;

            foreach (var sec in securityIds)
            {
                _sw.Restart();
                await _dbToTest.GetAvgPrices(start, end, accountCodes[i], sec);
                rm.AvgMs += _sw.ElapsedMilliseconds;
            }

            foreach (var sec in securityIds)
            {
                _sw.Restart();
                await _dbToTest.GetHoldingsLowerThan30DayAvg(start, end, accountCodes[i], sec);
                rm.LessThan30DAvgMs += _sw.ElapsedMilliseconds;
            }

            _sw.Restart();
            await _dbToTest.GetHighestAndLowestPrices(start, end, accountCodes[i]);
            rm.HighLowMs += _sw.ElapsedMilliseconds;

            Console.WriteLine($"{_dbToTest.GetType().FullName} : Run {i + 1} / {accountCodes.Length}. Ellapsed - Holdings:{rm.HoldingsMs}ms, Avg: {rm.AvgMs}, LT30DA: {rm.LessThan30DAvgMs}, HighLow: {rm.HighLowMs}");
        }

        return rm;




        DateTime startDate = DateTime.ParseExact("2012-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime endDate = DateTime.ParseExact("2012-06-30", "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var monthsToRunPerLoop = 7;
        var totalRuns = Math.Ceiling((decimal)accountCodes.Count() * (((endDate.Year - startDate.Year) * 12) + monthsToRunPerLoop + endDate.Month - startDate.Month) / monthsToRunPerLoop) - 4;
        int runCounter = 0;

        while (startDate <= endDate)
        {
            DateTime to = startDate.AddMonths(monthsToRunPerLoop).AddDays(-1);

            foreach (var accountCode in accountCodes)
            {
                var startEllapsed = _sw.ElapsedMilliseconds;
                _sw.Start();
                await _dbToTest.GetHoldings(startDate, to, accountCode);
                _sw.Stop();

                Console.WriteLine($"Run GetHoldings with {startDate.ToShortDateString()} - {to.ToShortDateString()}, Acc. {accountCode}. Ellapsed {_sw.ElapsedMilliseconds}ms");

                foreach (var securityId in securityIds)
                {
                    _sw.Start();
                    await _dbToTest.GetAvgPrices(startDate, to, accountCode, securityId);
                    _sw.Stop();

                    Console.WriteLine($"Run GetAvgPrices with {startDate.ToShortDateString()} - {to.ToShortDateString()}, Acc. {accountCode}, secId. {securityId}. Ellapsed {_sw.ElapsedMilliseconds}ms");
                }

                Console.WriteLine($"{_dbToTest.GetType().FullName} : Run {runCounter++} / {totalRuns}. Ellapsed: {_sw.ElapsedMilliseconds - startEllapsed}ms");
            }

            startDate = startDate.AddMonths(monthsToRunPerLoop).AddDays(-1);
        }

        Console.WriteLine($"Run all tests for {_dbToTest.GetType().FullName}, time: {_sw.ElapsedMilliseconds}ms");
        var ellapsed = _sw.ElapsedMilliseconds;
        _sw.Reset();

    }
}
