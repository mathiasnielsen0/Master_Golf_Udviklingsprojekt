internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Please enter PORT for API: ");
        var port = int.Parse(Console.ReadLine());
        var testRunner = new TestRunner(port);
        

        testRunner.RunTests("TimeScaleDB");
        testRunner.RunTests("MSSql");
        //testRunner.RunTests("InfluxDB");
    }
}