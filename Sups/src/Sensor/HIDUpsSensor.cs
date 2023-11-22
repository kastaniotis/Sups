using Iconic.Console;
using Iconic.Sups.HID;

namespace Iconic.Sups.Sensor;

public class HidUpsSensor : ISensor
{
    public Snapshot Read(string path)
    {
        var targetDevice = new FileInfo(path);
        var snapshot = new Snapshot();
        snapshot.Store("Device", path);
        try
        {
            using var fs = targetDevice.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var buffer = new byte[8];
            while (System.Console.KeyAvailable == false)
            {
                _ = fs.Read(buffer, 0, 8);
                var data = HidBatteryUsages.Parse(buffer);
                snapshot.Store(data.Key, data.Value);
                if (snapshot.IsComplete())
                {
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