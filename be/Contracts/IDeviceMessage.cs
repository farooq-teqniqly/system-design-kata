namespace Contracts;

public interface IDeviceMessage
{
    /// <summary>
    /// Gets or sets the unique identifier for the device.
    /// </summary>
    /// <value>The unique identifier for the device.</value>
    public string DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the type of the message.
    /// </summary>
    /// <value>The type of the message.</value>
    public string MessageType { get; set; }

    /// <summary>
    /// Gets or sets the version of the message.
    /// </summary>
    /// <value>The version of the message.</value>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the message.
    /// </summary>
    /// <value>The timestamp of the message.</value>
    public DateTimeOffset Timestamp { get; set; }
}
