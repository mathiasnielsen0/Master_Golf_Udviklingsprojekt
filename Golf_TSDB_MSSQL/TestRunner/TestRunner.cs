using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Globalization;

public class TestRunner
{
    private readonly string _baseUrl; // URL to the API
    private readonly HttpClient client;
    private readonly string[] accountCodes;
    private readonly int[] securityIds;

    public TestRunner(int port) 
    {
        _baseUrl = $"http://localhost:{port}";
        client = new HttpClient();
        
    
        // The 30 most frequent SecurityIds
        securityIds = new[]
            {
                1000600, 1002163, 1002185, 1002037, 1001032, 1007182, 1002243, 1007173,
                1001289, 1002521, 1007556, 1001277, 1001681, 1005940, 1003365, 1005267,
                1003364, 1003362, 1007369, 1003292, 1008544, 1002483, 1001924, 1001425,
                1003015, 1000941, 1001021, 1009221, 1003587, 1007682
            };

        // The 31 most frequent AccountCodes
        accountCodes = new[]
            {
                "1119", "1117", "1118", "1103", "1125", "1290", "1038", "1063", "1047",
                "1156", "1036", "1066", "1062", "1046", "1045", "1340", "1037", "1039",
                "1041", "1044", "1071", "1072", "1109", "1126", "1040", "1108", "1107",
                "1065", "1131", "1058", "1001"
            };
    }

    public void RunTests(string prefix)
    {
        var results = new List<string>();
        results.Add($"Start;End;AccountCode;; Time ms");
        var resultsAvg = new List<string>();
        resultsAvg.Add($"Start;End;AccountCode;SecurityId; Time ms");

        DateTime startDate = DateTime.ParseExact("2012-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DateTime endDate = DateTime.ParseExact("2014-11-30", "yyyy-MM-dd", CultureInfo.InvariantCulture);

        while (startDate <= endDate)
        {
            DateTime to = startDate.AddMonths(3).AddDays(-1);

            foreach (var accountCode in accountCodes)
            {
                // Hent og gem resultater for InfluxHoldings
                string responseHoldings = CallApi(prefix, startDate, to, accountCode);
                Console.Write($"\r{startDate};{to};{accountCode};; {responseHoldings}          ");
                results.Add($"{startDate};{to};{accountCode};; {responseHoldings}");

                foreach (var securityId in securityIds)
                {
                    // Hent og gem resultater for InfluxAverage
                    string responseAverage = CallApi(prefix, startDate, to, accountCode, securityId);
                    Console.Write($"\r{startDate:dd-MM-yyyy};{to:dd-MM-yyyy};{accountCode};{securityId}; {responseHoldings}          ");
                    resultsAvg.Add($"{startDate:dd-MM-yyyy};{to:dd-MM-yyyy};{accountCode};{securityId}; {responseAverage}");
                }
            }

            startDate = startDate.AddMonths(3);
        }
        Console.WriteLine();
        File.WriteAllLines($"output_{prefix}_{DateTime.Now.ToFileTime()}.csv", results);
        File.WriteAllLines($"output_avg_{prefix}_{DateTime.Now.ToFileTime()}.csv", resultsAvg);
    }

    private string CallApi(string endpoint, DateTime from, DateTime to, string accountCode, int? securityId = null)
    {
        string url = $"{_baseUrl}/{endpoint}/{from:yyyy-MM-dd}/{to:yyyy-MM-dd}/{accountCode}";

        if (securityId.HasValue)
        {
            url += $"/{securityId}";
        }

        var response = client.GetAsync(url).Result;
        return response.Content.ReadAsStringAsync().Result;
    }
}
