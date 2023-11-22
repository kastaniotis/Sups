using Iconic.Console;
using Iconic.Logger;
using Iconic.Sups.HID;

namespace Iconic.Sups.Sensor;

public class HidUpsSensor : ISensor
{
    private ILoggingService Logger { get; }

    public HidUpsSensor(ILoggingService logger)
    {
        Logger = logger;
    }

    public Snapshot Read(string path)
    {
        var targetDevice = new FileInfo(path);
        var snapshot = new Snapshot();
        Logger.Log("Sensor reading", path);
        snapshot.Store("Device", path);
        try
        {
            using var fs = targetDevice.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var buffer = new byte[8];
            while (System.Console.KeyAvailable == false)
            {
                _ = fs.Read(buffer, 0, 8);
                var data = HidBatteryUsages.Parse(buffer);
                Logger.Log("Received", BitConverter.ToString(buffer));
                snapshot.Store(data.Key, data.Value);
                Logger.Log(data.Key, data.Value);
                if (snapshot.IsComplete())
                {
                    Logger.Log("Snapshot is now Complete", snapshot);
                    return snapshot;
                }
            }
        }
        catch (Exception ex)
        {
            Terminal.WriteLineInRed(ex.Message);
            Environment.Exit(1);
        }

        return new Snapshot();
    }
}