using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MSSQL;

namespace Golf_TSDB_MSSQL.Controllers;

[ApiController]
[Route("[controller]")]
public class TsDb2Controller : ControllerBase
{
    private readonly IMyDbContext _myDbContext;

    public TsDb2Controller(IMyDbContext myDbContext)
    {
        
    }

    [HttpGet(Name = "Results3")]
    public IActionResult Results()
    {
        var sw = new Stopwatch();
        sw.Start();
        
        // TODO: Hent resultater
        
        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }
}