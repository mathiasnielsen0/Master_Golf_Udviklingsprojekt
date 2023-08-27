using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MSSQL;

namespace Golf_TSDB_MSSQL.Controllers;

[ApiController]
[Route("[controller]")]
public class MsSqlController : ControllerBase
{
    private readonly IMyDbContext _myDbContext;

    public MsSqlController(IMyDbContext myDbContext)
    {
        _myDbContext = myDbContext;
    }

    [HttpGet(Name = "Results1")]
    public IActionResult Results()
    {
        var sw = new Stopwatch();
        sw.Start();
        
        // TODO: Hent resultater
        var results = _myDbContext.HoldingsInAccounts
            .Take(1000)
            .ToList();
        
        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }
}