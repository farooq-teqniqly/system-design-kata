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

    /// <summary>
    /// Gets or sets the version of the message.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Gets or sets the timestamp of the message.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the temperature value.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the unit of the temperature value.
    /// </summary>
    public char Unit { get; set; }
}
