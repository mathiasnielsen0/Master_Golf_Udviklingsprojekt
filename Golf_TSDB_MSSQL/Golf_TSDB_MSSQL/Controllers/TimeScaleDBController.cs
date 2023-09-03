using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TSDB2.Data;

namespace Golf_TSDB_MSSQL.Controllers;

[ApiController]
[Route("[controller]")]
public class TimeScaleDBController : ControllerBase
{
    private readonly ITimeScaleDbContext _timeScaleDbContext;

    public TimeScaleDBController(ITimeScaleDbContext timeScaleDbContext)
    {
        _timeScaleDbContext = timeScaleDbContext;
    }

    [HttpGet(Name = "Results2")]
    public IActionResult Results()
    {
        var sw = new Stopwatch();
        sw.Start();

        var startDate = new DateTime(2014, 12, 1);
        
        // TODO: Hent resultater
        var results = _timeScaleDbContext.holdings_in_accounts.Max(x => x.navdate);
        
        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }
}