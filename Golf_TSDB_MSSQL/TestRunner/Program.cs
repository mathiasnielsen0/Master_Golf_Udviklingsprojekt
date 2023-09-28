internal class Program
{
    private static void Main(string[] args)
    {
        var testRunner = new TestRunner();

        testRunner.RunTests("TimeScaleDB");
        //testRunner.RunTests("InfluxDB");
        //testRunner.RunTests("MSSql");
    }
}