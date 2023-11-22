using Iconic.Logger;
using Iconic.Sups;

var monitor = new UpsMonitor(args);
monitor.Read();
monitor.Display();
monitor.Monitor();
