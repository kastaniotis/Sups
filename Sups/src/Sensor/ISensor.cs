namespace Iconic.Sups.Sensor;

public interface ISensor
{
    public Snapshot Read(string path);
}