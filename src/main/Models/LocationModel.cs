namespace ei8.Cortex.Gps.Sender.Models;

public record LocationModel(double latitude, double longitude, double bearing)
{
    public double Latitude { get; } = latitude;
    public double Longitude { get; } = longitude;
    public double Bearing { get; } = bearing;
}