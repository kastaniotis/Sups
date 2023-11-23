using Iconic.Sups;

var monitor = new UpsMonitor(args);
var snapshot = monitor.Read();
monitor.Display(snapshot);
monitor.Monitor(snapshot);
