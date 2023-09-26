using System.Diagnostics;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Golf_TSDB_MSSQL.Controllers;

[ApiController]
[Route("[controller]")]
public class MsSqlController : ControllerBase
{
    private readonly IDatabase mSSqlDatabase;

    public MsSqlController(MSSqlDatabase mSSqlDatabase)
    {
        this.mSSqlDatabase = mSSqlDatabase;
    }

    [HttpGet(Name = "Results1")]
    public IActionResult Results()
    {
        var sw = new Stopwatch();
        sw.Start();

        var startDate = new DateTime(2014, 12, 1);

        // TODO: Hent resultater
        var results = mSSqlDatabase.GetHoldings(startDate, startDate.AddMonths(1), "1080");
        
        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }

    [HttpGet("{from}/{to}/{accountCode}", Name = "MSSqlHoldings")]
    public async Task<IActionResult> Results45(DateTime from, DateTime to, string accountCode)
    {
        var sw = new Stopwatch();
        sw.Start();

        // TODO: Hent resultater
        var results = mSSqlDatabase.GetHoldings(from, to, accountCode);

        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }

    [HttpGet("{from}/{to}/{accountCode}/{securityId}", Name = "MSSqlAverage")]
    public async Task<IActionResult> Results45(DateTime from, DateTime to, string accountCode, int securityId)
    {
        var sw = new Stopwatch();
        sw.Start();

        // TODO: Hent resultater
        var results = mSSqlDatabase.GetAvgPrices(from, to, accountCode, securityId);

        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }


}