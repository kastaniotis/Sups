using Iconic.Sups.HID;

namespace Iconic.Sups;

public class Snapshot
{
    public int Charge { get; set; }
    public bool Charging { get; set; }
    public bool Discharging { get; set; }
    public bool Full { get; set; }
    public bool AcPresent { get; set; }
    public int Time { get; set; }
    public string Port { get; set; } = "";
    public bool Monitoring { get; set; }
    public int ShutdownThreshold { get; set; } 
    public List<string> Data { get; } = new();
    public string ChargerStatus { get; set; } = Iconic.Sups.ChargerStatus.Charging;

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