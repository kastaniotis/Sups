using Sups.HID;

namespace Sups;

public class Snapshot
{
    public Dictionary<string, object> Data { get; } = new();
    public bool IsComplete(){
        return
            Data.ContainsKey(HidBatteryUsages.ChargeText) &&
            Data.ContainsKey(HidBatteryUsages.ChargingText) &&
            Data.ContainsKey(HidBatteryUsages.DischargingText) &&
            Data.ContainsKey(HidBatteryUsages.FullText) &&
            Data.ContainsKey(HidBatteryUsages.AcPresentText) &&
            Data.ContainsKey(HidBatteryUsages.TimeText);
    }

    public void Store(KeyValuePair<string,object> pair)
    {
        Data[pair.Key] = pair.Value;
    }
}