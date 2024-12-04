using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace EdgeDevice;

public class MessageProducer<TMessage> : IMessageProducer<TMessage>
{
    private readonly ServiceBusSender _sender;
    private readonly ILogger<MessageProducer<TMessage>> _logger;
    private readonly Random _random = new Random();

    public MessageProducer(ServiceBusSender sender, ILogger<MessageProducer<TMessage>> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task SendMessageAsync(TMessage message, CancellationToken ctx=default)
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