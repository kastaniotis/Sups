namespace Sups.Sensor;

public interface ISensor
{
    public Snapshot Read(string path);
}