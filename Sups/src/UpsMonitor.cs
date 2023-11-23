using System.Diagnostics;
using Iconic.Console;
using Iconic.Logger;
using Iconic.Sups.Sensor;

namespace Iconic.Sups;

public class UpsMonitor
{
    private bool Debug { get; set; }
    private ILoggingService Logger { get; set; }
    private bool Json { get; }
    private string Port { get; } = "";
    private bool Monitoring { get; }
    private int ShutdownThreshold { get; } = 50;
    private Snapshot Data { get; set; } = new();

    public UpsMonitor(string[] args)
    {
        Debug = Arguments.Include(args, "--debug");
        Logger = new ConsoleLoggingService(Debug);
        Logger.Log("Logging is now", Debug);

        if (Arguments.Include(args, "--help"))
        {
            System.Console.WriteLine(@"
Syntax: 
$: sups

The UPS device must by HID compliant
            
Options are:
--help                  for this text 
--debug                 to see low level info about receiving and parsing HID UPS data 
--json                  to get results in json format.
--port /dev/usb/hiddev0 to connect to a custom port
--monitoring            to enable shutting down the local machine if charge goes below the threshold (default 50%)
--threshold 30          to define a custom shutdown threshold (in %)
");
            Environment.Exit(1);
        }

        Json = Arguments.Include(args, "--json");
        Logger.Log("Json is now", Json);

        // If no port defined, we scan
        if (!Arguments.Include(args, "--port"))
        {
            const string path = "/dev";
            Logger.Log("No port was specified. Trying to Detect in", path);

            string?[] devs = Directory.GetFiles(path, "hiddev*", SearchOption.AllDirectories);
            Array.Sort(devs);
            if (devs.Length > 0)
            {
                Port = devs[0] ?? string.Empty;
                Logger.Log("Device Found", Port);
            }
            else
            {
                Terminal.WriteLineInRed(
                    $"Could not detect any hiddev devices in {path}. Please use the --port argument.");
                Environment.Exit(1);
            }
        }
        else
        {
            var portResult = Arguments.GetStringValueResult(args, "--port");
            if (portResult.Success)
            {
                Port = portResult.Data ?? string.Empty;
                Logger.Log("Port was specified in arguments", Port);
            }
            else
            {
                Terminal.WriteLineInRed(portResult.Message);
                Environment.Exit(1);
            }
        }

        Monitoring = Arguments.Include(args, "--monitoring");
        Logger.Log("Monitoring is now", Monitoring);

        var thresholdResult = Arguments.GetIntValueResult(args, "--threshold");
        if (thresholdResult is { Success: true, Data: not null })
        {
            ShutdownThreshold = (int)thresholdResult.Data;
            Logger.Log("Shutdown Threshold is specified", ShutdownThreshold);
        }
    }

    public string ChargerStatus(Snapshot snapshot)
    {
        if (snapshot.Full)
        {
            return Status.Full;
        }

        if (snapshot.Discharging)
        {
            return Status.Discharging;
        }

        if (snapshot.Charging)
        {
            return Status.Charging;
        }

        return Status.Check;
    }

    public Snapshot Read()
    {
        Logger.Log("Trying to read device at", Port);
        var sensor = new HidUpsSensor(Logger);
        Data = sensor.Read(Port);
        Data.Store("Port", Port);
        Data.Store("Monitoring", Monitoring);
        Data.Store("ShutdownThreshold", ShutdownThreshold);
        Data.Store("Status", ChargerStatus(Data));
        Logger.Log("Device Status is now", ChargerStatus(Data));
        return Data;
    }

    public void Monitor(Snapshot snapshot)
    {
        Logger.Log("Monitoring Battery Level", Monitoring);
        Logger.Log("Battery Charge Level", $"{snapshot.Charge} < {snapshot.ShutdownThreshold}?");
        if (Monitoring && !snapshot.AcPresent && snapshot.Charge < snapshot.ShutdownThreshold)
        {
            Logger.Log("Shutting down the local machine", string.Empty);
            var process = new Process();
            process.StartInfo.FileName = "shutdown";
            process.StartInfo.Arguments = "-h now";
            process.Start();
        }
    }

    public void Display(Snapshot snapshot)
    {
        if (Json)
        {
            Logger.Log("Printing Out Json", snapshot);
            Output.WriteJson(snapshot);
        }
        else
        {
            Logger.Log("Printing Out Table", snapshot);
            Output.WriteTable(snapshot);
        }
    }
}