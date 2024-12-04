namespace EdgeDevice;

public class TemperatureMessage
{
    /// <summary>
    /// Gets or sets the device ID.
    /// </summary>
    public required string DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the temperature value.
    /// </summary>
    public double Value { get; set; }
}
