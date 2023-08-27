using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Golf_TSDB_MSSQL.Controllers;

[ApiController]
[Route("[controller]")]
public class TsDb2Controller : ControllerBase
{
    public TsDb2Controller()
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