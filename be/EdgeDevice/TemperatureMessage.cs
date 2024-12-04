namespace EdgeDevice;

public class TemperatureMessage
{
    public required string DeviceId { get; set; }
    public double Value { get; set; }
}