using Iconic.Console;
using Sups;
using Sups.Sensor;

var monitor = new UpsMonitor(args);

var sensor = new HidUpsSensor();
var snapshot = sensor.Read(monitor.Port);
snapshot.Store(new KeyValuePair<string, object>("Port", monitor.Port));
snapshot.Store(new KeyValuePair<string, object>("Monitoring", monitor.Monitoring));
snapshot.Store(new KeyValuePair<string, object>("ShutdownThreshold", monitor.ShutdownThreshold));

if (monitor.Json)
{
    Output.WriteJson(snapshot);
}
else
{
    Output.WriteTable(snapshot);
}
