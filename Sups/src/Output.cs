using Iconic.Console;

namespace Iconic.Sups;

public static class Output
{
    public static void WriteTable(Snapshot snapshot)
    {
        System.Console.WriteLine($"Date:\t\t\t{DateTime.Now:s}");
        System.Console.WriteLine($"Port:\t\t\t{snapshot.Port}");
        Terminal.PrintIntInColor("Battery:\t\t", Convert.ToInt32(snapshot.Charge), snapshot.ShutdownThreshold, 100, "%");
        Terminal.PrintState("Status:\t\t\t", snapshot.ChargerStatus, Status.Full, Status.Charging, Status.Discharging);
        Terminal.PrintBoolInColor("AC Present:\t\t", snapshot.AcPresent, true, false);
        System.Console.WriteLine("Time Left:\t\t" + snapshot.Time + "'");
        System.Console.WriteLine("Shutdown At:\t\t" + snapshot.ShutdownThreshold + "%");
        Terminal.PrintBoolInColor("Local Monitoring:\t", snapshot.LocalMonitoring, true, false);
        if(snapshot.RemoteMonitoring != string.Empty){
            System.Console.WriteLine("Remote Monitoring:\t" + snapshot.RemoteMonitoring);
        }
    }


    public static string WriteJson(Snapshot snapshot)
    {
        var output = $$"""
                       {
                           "Date": "{{DateTime.Now:s}}",
                           "Port": "{{snapshot.Port}}",
                           "Charge": {{snapshot.Charge}},
                           "ACPresent": {{snapshot.AcPresent.ToString().ToLower()}},
                           "Time": {{snapshot.Time}},
                           "ChargerStatus": "{{snapshot.ChargerStatus}}",
                           "ShutdownThreshold": {{snapshot.ShutdownThreshold}},
                           "Monitoring": {{snapshot.LocalMonitoring.ToString().ToLower()}},
                           "RemoteMonitoring": "{{snapshot.RemoteMonitoring}}"
                       }
                       """;
        System.Console.WriteLine(output);
        return output;
    }
}