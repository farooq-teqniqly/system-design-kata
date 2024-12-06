using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace EdgeDevice;

/// <summary>
/// Represents a message producer that sends messages to a Service Bus.
/// </summary>
/// <typeparam name="TMessage">The type of the message to send.</typeparam>
public class MessageProducer<TMessage> : IMessageProducer<TMessage>
{
    private readonly ServiceBusSender _sender;
    private readonly ILogger<MessageProducer<TMessage>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageProducer{TMessage}"/> class.
    /// </summary>
    /// <param name="sender">The Service Bus sender.</param>
    /// <param name="logger">The logger.</param>
    public MessageProducer(ServiceBusSender sender, ILogger<MessageProducer<TMessage>> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    /// <summary>
    /// Sends a message asynchronously.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="ctx">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendMessageAsync(TMessage message, CancellationToken ctx = default)
    {
        if (ctx.IsCancellationRequested)
        {
            return;
        }

        var messageBody = JsonSerializer.Serialize(message);
        var messageBytes = Encoding.UTF8.GetBytes(messageBody);
        var sbMessage = new ServiceBusMessage(messageBytes) { ContentType = "application/json" };

        try
        {
            await _sender.SendMessageAsync(sbMessage, ctx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error sending message");
            throw;
        }
    }
}
