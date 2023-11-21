using Iconic.Console;
using Sups.HID;

namespace Sups;

public class Output
{
    public static void WriteTable(Snapshot snapshot)
    {
        Console.WriteLine($"Port:\t\t{snapshot.Data["Port"]}");
        Terminal.PrintIntInColor("Charge", Convert.ToInt32(snapshot.Data[HidBatteryUsages.ChargeText]), (int)snapshot.Data["ShutdownThreshold"], 100, "%");
        var status = UpsMonitor.ChargerStatus((bool)snapshot.Data[HidBatteryUsages.FullText], (bool)snapshot.Data["Discharging"], (bool)snapshot.Data["Charging"]);
        Terminal.PrintState("Charger Status", status, HidBatteryChargerStatus.Charged, HidBatteryChargerStatus.Charging, HidBatteryChargerStatus.Discharging);
        Terminal.PrintBoolInColor("AC Present", (bool)snapshot.Data[HidBatteryUsages.AcPresentText], true, false);
        Console.WriteLine("Time:\t\t" + snapshot.Data[HidBatteryUsages.TimeText] + "'");
        Console.WriteLine("Shutdown:\t" + snapshot.Data["ShutdownThreshold"] + "%");
        Terminal.PrintBoolInColor("Monitoring", (bool)snapshot.Data["Monitoring"], true, false);
    }

    public static void WriteJson(Snapshot snapshot)
    {
        Console.WriteLine(@$"{{
    ""Port"": ""{snapshot.Data["Port"]}"", 
    ""Charge"": {snapshot.Data[HidBatteryUsages.ChargeText]},
    ""ACPresent"": {snapshot.Data[HidBatteryUsages.AcPresentText].ToString().ToLower()},
    ""Time"": {snapshot.Data[HidBatteryUsages.TimeText]},
    ""ChargerStatus"": ""{UpsMonitor.ChargerStatus((bool)snapshot.Data[HidBatteryUsages.FullText], (bool)snapshot.Data["Discharging"], (bool)snapshot.Data["Charging"])}"",
    ""ShutdownThreshold"": {snapshot.Data["ShutdownThreshold"]},
    ""Monitoring"": {snapshot.Data["Monitoring"].ToString().ToLower()}
}}");
    }
}