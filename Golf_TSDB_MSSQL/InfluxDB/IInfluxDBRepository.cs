using Core.Models;
using InfluxDB.Client.Writes;

namespace InfluxDB
{
    public interface IInfluxDBRepository
    {
        Task<string> QueryDataAsync(string bucket, string org, string query);
        Task<List<HoldingsInAccount>> QueryDataAsync(string bucket, string org, string accountCode, DateTime navDate);
        void WriteDataAsync(string bucket, string org, PointData point);
        void WriteDataAsync(string bucket, string org, HoldingsInAccount holdingInAccount);
        void WriteDataBatchAsync(string bucket, string org, List<Core.Models.HoldingsInAccount> records);
    }
}