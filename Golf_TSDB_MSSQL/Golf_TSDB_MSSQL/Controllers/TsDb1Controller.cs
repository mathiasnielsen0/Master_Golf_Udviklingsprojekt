using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Golf_TSDB_MSSQL.Controllers;

[ApiController]
[Route("[controller]")]
public class TsDb1Controller : ControllerBase
{
    public TsDb1Controller()
    {
    }

    [HttpGet(Name = "Results2")]
    public IActionResult Results()
    {
        var sw = new Stopwatch();
        sw.Start();
        
        // TODO: Hent resultater
        
        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }
}