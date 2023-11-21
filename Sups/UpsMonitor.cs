using Iconic.Console;

namespace Sups;

public class UpsMonitor
{
    public bool Debug { get; set; }
    public bool Help { get; set; }
    public bool Json { get; set; }
    public string Port { get; set; } = "";
    public bool Monitoring { get; set; }
    public int ShutdownThreshold { get; set; } = 50;

    public UpsMonitor(string[] args)
    {
        Debug = Arguments.Include(args, "--debug");
        
        if(Arguments.Include(args, "--help")){
            Console.WriteLine(@"
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
            Console.WriteLine("");
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
                Port = devs[0];
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
                Port = portResult.Data;
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
    
    public static string ChargerStatus(bool full, bool discharging, bool charging) {
            if(full){
                return HidBatteryChargerStatus.Charged;
            }

            if (discharging){
                return HidBatteryChargerStatus.Discharging;
            }

            if (charging){
                return HidBatteryChargerStatus.Charging;
            }

            return HidBatteryChargerStatus.Check;
    }
}