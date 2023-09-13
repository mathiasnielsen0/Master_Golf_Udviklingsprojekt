using Core.Models;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;

namespace InfluxDB
{
    public interface IInfluxDBRepository
    {
        Task<string> QueryDataAsync(string bucket, string org, string query);
        Task<FluxRecord> QueryDataOneRecordAsync(string bucket, string org, string query);
        Task<List<FluxRecord>> QueryDataMultipleRecordsAsync(string bucket, string org, string query);
        Task<List<HoldingsInAccount>> QueryDataAsync(string bucket, string org, string accountCode, DateTime navDate);
        void WriteDataAsync(string bucket, string org, PointData point);
        void WriteDataAsync(string bucket, string org, List<PointData> point);
        void WriteDataAsync(string bucket, string org, HoldingsInAccount holdingInAccount);
        void WriteDataBatchAsync(string bucket, string org, List<Core.Models.HoldingsInAccount> records);
        Task<int> GetRowCountAsync(string bucket, string org);

    }
}