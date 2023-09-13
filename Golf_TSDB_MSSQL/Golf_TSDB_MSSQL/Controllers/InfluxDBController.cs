using System.Diagnostics;
using InfluxDB;
using Microsoft.AspNetCore.Mvc;
using MSSQL;

namespace Golf_TSDB_MSSQL.Controllers;

[ApiController]
[Route("[controller]")]
public class InfluxDBController : ControllerBase
{
    private readonly IMyDbContext _myDbContext;
    private readonly IInfluxDBRepository influxDBRepository;

    public InfluxDBController(IMyDbContext myDbContext, IInfluxDBRepository influxDBRepository)
    {
        this.influxDBRepository = influxDBRepository;
    }

    [HttpGet(Name = "Results4")]
    public async Task<IActionResult> Results()
    {
        var sw = new Stopwatch();
        sw.Start();

        var qr = await influxDBRepository.QueryDataAsync("Holdings", "Sparinvest", "1098", new DateTime(2012, 1, 2));

        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }
}