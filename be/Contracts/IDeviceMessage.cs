namespace Contracts;

public interface IDeviceMessage
{
    /// <summary>
    /// Gets or sets the unique identifier for the device.
    /// </summary>
    public string DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the type of the message.
    /// </summary>
    public string MessageType { get; set; }

    public int Version { get; set; }
}
