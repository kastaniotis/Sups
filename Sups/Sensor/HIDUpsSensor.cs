using Iconic.Console;
using Sups.HID;

namespace Sups.Sensor;

public class HidUpsSensor : ISensor
{
    public Snapshot Read(string path)
    {
        var targetDevice = new FileInfo(path);
        var snapshot = new Snapshot();
        snapshot.Store(new KeyValuePair<string, object>("Device", path));
        try
        {
            using (FileStream fs = targetDevice.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var buffer = new byte[8];
                while (Console.KeyAvailable == false)
                {
                    fs.Read(buffer, 0, 8);
                    //Console.WriteLine(HidBatteryUsages.Parse(buffer));
                    snapshot.Store(HidBatteryUsages.Parse(buffer));
                    if (snapshot.IsComplete())
                    {
                        return snapshot;
                    }
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