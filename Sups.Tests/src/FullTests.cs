using System.Text.Json.Nodes;
using Iconic.Sups;
using Iconic.Sups.HID;

namespace Sups.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestParseFull()
    {
        var snapshot = new Snapshot();
        
        byte[] timeBuffer = { 0x68, 0x00, 0x85, 0x00, 0x60, 0x09, 0x00, 0x00 };
        var timeResult = HidBatteryUsages.Parse(timeBuffer);
        Assert.That(timeResult.Key, Is.EqualTo(HidBatteryUsages.TimeText));
        Assert.That(timeResult.Value, Is.EqualTo(40));
        
        snapshot.Store(timeResult.Key, timeResult.Value);
        Assert.That(snapshot.IsComplete(), Is.EqualTo(false));
        
        byte[] ignoredBuffer = { 0x42, 0x00, 0x85, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var ignoredResult = HidBatteryUsages.Parse(ignoredBuffer);
        Assert.That(ignoredResult.Key, Is.EqualTo(HidBatteryUsages.IgnoredText));
        Assert.That(ignoredResult.Value, Is.EqualTo("42: 00-00"));
        
        snapshot.Store(ignoredResult.Key, ignoredResult.Value);
        Assert.That(snapshot.IsComplete(), Is.EqualTo(false));

        byte[] chargeBuffer = { 0x66, 0x00, 0x85, 0x00, 0x64, 0x00, 0x00, 0x00 };
        var chargeResult = HidBatteryUsages.Parse(chargeBuffer);
        Assert.That(chargeResult.Key, Is.EqualTo(HidBatteryUsages.ChargeText));
        Assert.That(chargeResult.Value, Is.EqualTo(100));
        
        snapshot.Store(chargeResult.Key, chargeResult.Value);
        Assert.That(snapshot.IsComplete(), Is.EqualTo(false));
    
        byte[] chargingBuffer = { 0x44, 0x00, 0x85, 0x00, 0x01, 0x00, 0x00, 0x00 };
        var chargingResult = HidBatteryUsages.Parse(chargingBuffer);
        Assert.That(chargingResult.Key, Is.EqualTo(HidBatteryUsages.ChargingText));
        Assert.That(chargingResult.Value, Is.EqualTo(true));
        
        snapshot.Store(chargingResult.Key, chargingResult.Value);
        Assert.That(snapshot.IsComplete(), Is.EqualTo(false));
        
        byte[] acBuffer = { 0xD0, 0x00, 0x85, 0x00, 0x01, 0x00, 0x00, 0x00 };
        var acResult = HidBatteryUsages.Parse(acBuffer);
        Assert.That(acResult.Key, Is.EqualTo(HidBatteryUsages.AcPresentText));
        Assert.That(acResult.Value, Is.EqualTo(true));
        
        snapshot.Store(acResult.Key, acResult.Value);
        Assert.That(snapshot.IsComplete(), Is.EqualTo(false));
    
        byte[] dischargingBuffer = { 0x45, 0x00, 0x85, 0x00, 0x00, 0x00, 0x00, 0x00 };
        var dischargingResult = HidBatteryUsages.Parse(dischargingBuffer);
        Assert.That(dischargingResult.Key, Is.EqualTo(HidBatteryUsages.DischargingText));
        Assert.That(dischargingResult.Value, Is.EqualTo(false));
        
        snapshot.Store(dischargingResult.Key, dischargingResult.Value);
        Assert.That(snapshot.IsComplete(), Is.EqualTo(false));

        byte[] fullBuffer = { 0x46, 0x00, 0x85, 0x00, 0x01, 0x00, 0x00, 0x00 };
        var fullResult = HidBatteryUsages.Parse(fullBuffer);
        Assert.That(fullResult.Key, Is.EqualTo(HidBatteryUsages.FullText));
        Assert.That(fullResult.Value, Is.EqualTo(true));
    
        snapshot.Store(fullResult.Key, fullResult.Value);
        
        snapshot.Store("Port", "/dev/usb/hiddev0");
        snapshot.Store("ShutdownThreshold", 35);
        snapshot.Store("Monitoring", true);
        
        Assert.That(snapshot.IsComplete(), Is.EqualTo(true));
        
        var json = Output.WriteJson(snapshot);
        
        var encoded = JsonNode.Parse(json);
    }
}