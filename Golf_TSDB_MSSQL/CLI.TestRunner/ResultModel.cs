namespace CLI.TestRunner;

public class ResultModel
{
    public string DbType { get; set; }
    public long HoldingsMs { get; set; }    
    public long AvgMs { get; set; }    
    public long LessThan30DAvgMs { get; set; }
}