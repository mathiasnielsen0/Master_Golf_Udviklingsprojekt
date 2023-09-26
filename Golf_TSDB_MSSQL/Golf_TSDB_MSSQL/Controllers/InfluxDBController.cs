using System.Diagnostics;
using Core.Interfaces;
using InfluxDB;
using InfluxDB.Client.Api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Golf_TSDB_MSSQL.Controllers;

[ApiController]
[Route("[controller]")]
public class InfluxDBController : ControllerBase
{
    private readonly IDatabase influxDBRepository;

    public InfluxDBController(InfluxDBRepository influxDBRepository)
    {
        this.influxDBRepository = influxDBRepository;
    }
   

    [HttpGet("{from}/{to}/{accountCode}", Name = "InfluxHoldings")]
    public IActionResult Results25(DateTime from, DateTime to, string accountCode)
    {
        var sw = new Stopwatch();
        sw.Start();

        // TODO: Hent resultater
        var results = influxDBRepository.GetHoldings(from, to, accountCode);

        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }

    [HttpGet("{from}/{to}/{accountCode}/{securityId}", Name = "InfluxAverage")]
    public IActionResult Results45(DateTime from, DateTime to, string accountCode, int securityId)
    {
        var sw = new Stopwatch();
        sw.Start();

        // TODO: Hent resultater
        var results = influxDBRepository.GetAvgPrices(from, to, accountCode, securityId);

        var ellapsed = sw.ElapsedMilliseconds;
        return Content(ellapsed.ToString());
    }
}