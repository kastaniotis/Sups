using Iconic.Console;

namespace Iconic.Sups;

public static class Output
{
    public static void WriteTable(Snapshot snapshot)
    {
        System.Console.WriteLine($"Port:\t\t{snapshot.Port}");
        Terminal.PrintIntInColor("Battery:\t", Convert.ToInt32(snapshot.Charge), snapshot.ShutdownThreshold, 100, "%");
        Terminal.PrintState("Status:\t\t", snapshot.ChargerStatus, Status.Full, Status.Charging, Status.Discharging);
        Terminal.PrintBoolInColor("AC Present:\t", snapshot.AcPresent, true, false);
        System.Console.WriteLine("Time Left:\t" + snapshot.Time + "'");
        System.Console.WriteLine("Shutdown At:\t" + snapshot.ShutdownThreshold + "%");
        Terminal.PrintBoolInColor("Monitoring:\t", snapshot.Monitoring, true, false);
    }

    public static void WriteJson(Snapshot snapshot)
    {
        System.Console.WriteLine(@$"{{
    ""Port"": ""{snapshot.Port}"", 
    ""Charge"": {snapshot.Charge},
    ""ACPresent"": {snapshot.AcPresent.ToString().ToLower()},
    ""Time"": {snapshot.Time},
    ""ChargerStatus"": ""{snapshot.ChargerStatus}"",
    ""ShutdownThreshold"": {snapshot.ShutdownThreshold},
    ""Monitoring"": {snapshot.Monitoring.ToString().ToLower()}
}}");
    }
}