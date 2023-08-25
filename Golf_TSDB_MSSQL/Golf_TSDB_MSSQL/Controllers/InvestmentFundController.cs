using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Golf_TSDB_MSSQL.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetMssqlResults")]
    public IActionResult GetMssqlResults()
    {
        var sw = new Stopwatch();
        sw.Start();
        
        // TODO: Hent resultater
        
        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }
    
    [HttpGet(Name = "GetTsdb1Results")]
    public IActionResult GetTsdb1Results()
    {
        var sw = new Stopwatch();
        sw.Start();
        
        // TODO: Hent resultater
        
        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }
    
    [HttpGet(Name = "GetTsdb2Results")]
    public IActionResult GetTsdb2Results()
    {
        var sw = new Stopwatch();
        sw.Start();
        
        // TODO: Hent resultater
        
        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }
}