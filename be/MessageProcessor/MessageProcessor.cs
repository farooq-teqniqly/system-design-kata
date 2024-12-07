using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace MessageProcessor;

/// <summary>
/// Processes messages received from Azure Service Bus.
/// </summary>
public class MessageProcessor : IMessageProcessor
{
    private readonly ILogger<MessageProcessor> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
    /// </summary>
    /// <param name="logger">The logger instance to log messages.</param>
    public MessageProcessor(ILogger<MessageProcessor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Processes the received message asynchronously.
    /// </summary>
    /// <param name="message">The received Service Bus message.</param>
    /// <param name="ctx">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ProcessMessageAsync(ServiceBusReceivedMessage message, CancellationToken ctx = default)
    {
        _logger.LogDebug("Received message @{message} with body @{body} and enqueue time @{enqueueTime}", message, message.Body, message.EnqueuedTime);

        var _ = message.Body.ToString();

        return Task.CompletedTask;
    }
}
