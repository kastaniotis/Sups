using Iconic.Console;

namespace Iconic.Sups;

public static class Output
{
    public static void WriteTable(Snapshot snapshot)
    {
        System.Console.WriteLine($"Port:\t\t{snapshot.Port}");
        Terminal.PrintIntInColor("Charge", Convert.ToInt32(snapshot.Charge), snapshot.ShutdownThreshold, 100, "%");
        Terminal.PrintState("Charger Status", snapshot.ChargerStatus, HidBatteryChargerStatus.Charged, HidBatteryChargerStatus.Charging, HidBatteryChargerStatus.Discharging);
        Terminal.PrintBoolInColor("AC Present", snapshot.AcPresent, true, false);
        System.Console.WriteLine("Time:\t\t" + snapshot.Time + "'");
        System.Console.WriteLine("Shutdown:\t" + snapshot.ShutdownThreshold + "%");
        Terminal.PrintBoolInColor("Monitoring", snapshot.Monitoring, true, false);
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