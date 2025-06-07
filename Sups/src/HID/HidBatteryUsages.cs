namespace Iconic.Sups.HID;

public struct HidBatteryUsages
{
    // https://www.usb.org/sites/default/files/hut1_4.pdf#page=376
    // private const string Battery = "85";
    // private const string RemainingTimeLimit = "2A";
    // private const string BelowRemainingCapacityLimit = "42";
    // private const string RemainingTimeLimitExpired = "43";
    public const string IgnoredText = "Ignored";

    private const string Charging = "44";
    public const string ChargingText = "Charging";
    private const string Discharging = "45";
    public const string DischargingText = "Discharging";
    private const string Full = "46";
    public const string FullText = "Full";
    private const string Time = "68";
    public const string TimeText = "Time";
    private const string Charge = "66";
    public const string ChargeText = "Charge";
    private const string AcPresent = "D0";
    public const string AcPresentText = "AcPresent";

    public static KeyValuePair<string, object> Parse(byte[] buffer)
    {
        //var subject = BitConverter.ToString(buffer[2..3]); // Should always be 85 for HID ups reporting
        var property = BitConverter.ToString(buffer[..1]);
        var value = buffer[4..6];

        // Ignoring BelowRemainingCapacityLimit, RemainingTimeLimitExpired, Battery, RemainingTimeLimit
        return property switch
        {
            Time => new KeyValuePair<string, object>(TimeText, BitConverter.ToInt16(value) / 60), // Convert to Minutes
            Charge => new KeyValuePair<string, object>(ChargeText, BitConverter.ToInt16(value)),
            Charging => new KeyValuePair<string, object>(ChargingText, BitConverter.ToBoolean(value)),
            Discharging => new KeyValuePair<string, object>(DischargingText, BitConverter.ToBoolean(value)),
            Full => new KeyValuePair<string, object>(FullText, BitConverter.ToBoolean(value)),
            AcPresent => new KeyValuePair<string, object>(AcPresentText, BitConverter.ToBoolean(value)),
            _ => new KeyValuePair<string, object>(IgnoredText, $"{property}: {BitConverter.ToString(value)}")
        };
    }
}