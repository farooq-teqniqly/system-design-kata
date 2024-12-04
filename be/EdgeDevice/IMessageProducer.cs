namespace EdgeDevice;
/// <summary>
/// Represents a message producer that can send messages of type TMessage.
/// </summary>
/// <typeparam name="TMessage">The type of the message to be sent.</typeparam>
public interface IMessageProducer<in TMessage>
{
    /// <summary>
    /// Sends a message asynchronously.
    /// </summary>
    /// <param name="message">The message to be sent.</param>
    /// <param name="ctx">The cancellation token (optional).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendMessageAsync(TMessage message, CancellationToken ctx = default);
}
