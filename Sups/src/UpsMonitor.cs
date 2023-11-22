using System.Diagnostics;
using Iconic.Console;
using Iconic.Sups.Sensor;

namespace Iconic.Sups;

public class UpsMonitor
{
    public bool Debug { get; set; }
    private bool Json { get; }
    private string Port { get; } = "";
    private bool Monitoring { get; }
    private int ShutdownThreshold { get; } = 50;
    private Snapshot Data { get; set; } = new();

    public UpsMonitor(string[] args)
    {
        Debug = Arguments.Include(args, "--debug");
        
        if(Arguments.Include(args, "--help")){
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
        Monitoring = Arguments.Include(args, "--monitoring");
        
        // If no port defined, we scan
        if (!Arguments.Include(args, "--port"))
        {
            const string path = "/dev";
            string?[] devs = Directory.GetFiles(path, "hiddev*", SearchOption.AllDirectories);
            Array.Sort(devs);
            if(devs.Length > 0)
            {
                Port = devs[0] ?? string.Empty;
            }
            else{
                Terminal.WriteLineInRed($"Could not detect any hiddev devices in {path}. Please use the --port argument.");
                Environment.Exit(1);
            }
        }
        else
        {
            var portResult = Arguments.GetStringValueResult(args, "--port");
            if(portResult.Success){
                Port = portResult.Data ?? string.Empty;
            }
            else
            {
                Terminal.WriteLineInRed(portResult.Message);
                Environment.Exit(1);
            }
        }
        
        var thresholdResult = Arguments.GetIntValueResult(args, "--threshold");
        if(thresholdResult is { Success: true, Data: not null })
        {
            ShutdownThreshold = (int)thresholdResult.Data;
        }
    }

    private string ChargerStatus() {
            if(Data.Full){
                return Status.Full;
            }

            if (Data.Discharging){
                return Status.Discharging;
            }

            if (Data.Charging){
                return Status.Charging;
            }

            return Status.Check;
    }

    public void Read()
    {
        var sensor = new HidUpsSensor();
        Data = sensor.Read(Port);
        Data.Store("Port", Port);
        Data.Store("Monitoring", Monitoring);
        Data.Store("ShutdownThreshold", ShutdownThreshold);
        Data.Store("Status", ChargerStatus());
    }

    public void Monitor()
    {
        if (!Monitoring || Data.Charge >= Data.ShutdownThreshold) return;
        var process = new Process();
        process.StartInfo.FileName = "shutdown";
        process.StartInfo.Arguments = "-h now";
        process.Start();
    }

    public void Display()
    {
        if (Json)
        {
            Output.WriteJson(Data);
        }
        else
        {
            Output.WriteTable(Data);
        }
    }
}