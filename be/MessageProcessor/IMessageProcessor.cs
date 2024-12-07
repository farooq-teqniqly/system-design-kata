using Azure.Messaging.ServiceBus;

namespace MessageProcessor;
/// <summary>
/// Defines a contract for processing messages received from Azure Service Bus.
/// </summary>
public interface IMessageProcessor
{
    /// <summary>
    /// Processes a received message asynchronously.
    /// </summary>
    /// <param name="message">The received message from Azure Service Bus.</param>
    /// <param name="ctx">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ProcessMessageAsync(ServiceBusReceivedMessage message, CancellationToken ctx = default);
}
