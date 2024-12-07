namespace Contracts;

public class TemperatureMessage : IDeviceMessage
{
    /// <summary>
    /// Gets or sets the device ID.
    /// </summary>
    public required string DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the message type.
    /// </summary>
    public string MessageType { get; set; } = "temperature";

    public int Version { get; set; } = 1;

    /// <summary>
    /// Gets or sets the temperature value.
    /// </summary>
    public double Value { get; set; }
}
