using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TSDB2;
using TSDB2.Data;

namespace Golf_TSDB_MSSQL.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeScaleDBController : ControllerBase
{
    private readonly TimeScaleDb _timeScaleDbContext;

    public TimeScaleDBController(TimeScaleDb timeScaleDbContext)
    {
        _timeScaleDbContext = timeScaleDbContext;
    }

    [HttpGet("{from}/{to}/{accountCode}", Name = "TimescaleHoldings")]
    public async Task<IActionResult> Results45(DateTime from, DateTime to, string accountCode)
    {
        var sw = new Stopwatch();
        sw.Start();

        // TODO: Hent resultater
        var results = await _timeScaleDbContext.GetHoldings(from, to, accountCode);

        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }

    [HttpGet("{from}/{to}/{accountCode}/{securityId}", Name = "TimescaleAverage")]
    public async Task<IActionResult> Results45(DateTime from, DateTime to, string accountCode, int securityId)
    {
        var sw = new Stopwatch();
        sw.Start();

        // TODO: Hent resultater
        var results =await  _timeScaleDbContext.GetAvgPrices(from, to, accountCode, securityId);

        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }

}