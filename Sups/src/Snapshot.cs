using Iconic.Sups.HID;

namespace Iconic.Sups;

public class Snapshot
{
    public int Charge { get; private set; }
    public bool Charging { get; private set; }
    public bool Discharging { get; private set; }
    public bool Full { get; private set; }
    public bool AcPresent { get; private set; }
    public int Time { get; private set; }
    public string Port { get; private set; } = "";
    public bool Monitoring { get; private set; }
    public int ShutdownThreshold { get; private set; }
    private List<string> Data { get; } = new();
    public string ChargerStatus { get; private set; } = Status.Charging;

    public bool IsComplete()
    {
        return
            Data.Contains(HidBatteryUsages.ChargeText) &&
            Data.Contains(HidBatteryUsages.ChargingText) &&
            Data.Contains(HidBatteryUsages.DischargingText) &&
            Data.Contains(HidBatteryUsages.FullText) &&
            Data.Contains(HidBatteryUsages.AcPresentText) &&
            Data.Contains(HidBatteryUsages.TimeText);
    }

    public void Store(string key, object value)
    {
        Data.Add(key);
        switch (key)
        {
            case HidBatteryUsages.ChargeText:
                Charge = (short)value;
                break;
            case HidBatteryUsages.ChargingText:
                Charging = (bool)value;
                break;
            case HidBatteryUsages.DischargingText:
                Discharging = (bool)value;
                break;
            case HidBatteryUsages.FullText:
                Full = (bool)value;
                break;
            case HidBatteryUsages.AcPresentText:
                AcPresent = (bool)value;
                break;
            case HidBatteryUsages.TimeText:
                Time = (int)value;
                break;
            case "Port":
                Port = value.ToString() ?? string.Empty;
                break;
            case "Monitoring":
                Monitoring = (bool)value;
                break;
            case "ShutdownThreshold":
                ShutdownThreshold = (int)value;
                break;
            case "Status":
                ChargerStatus = value as string ?? string.Empty;
                break;
        }
    }
}
