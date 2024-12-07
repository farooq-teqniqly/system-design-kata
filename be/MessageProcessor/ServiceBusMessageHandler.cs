using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace MessageProcessor;

public class ServiceBusMessageHandler
{
    private readonly IMessageProcessor _messageProcessor;
    private readonly ILogger<ServiceBusMessageHandler> _logger;
    private readonly Action _criticalExceptionHandler;
    private readonly Action _defaultCriticalExceptionHandler = () => Environment.Exit(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusMessageHandler"/> class.
    /// </summary>
    /// <param name="messageProcessor">The message processor to handle incoming messages.</param>
    /// <param name="logger">The logger to log information and errors.</param>
    /// <param name="criticalExceptionHandler">Defines an Action that specifies how critical exceptions are to be handled.</param>
    public ServiceBusMessageHandler(
        IMessageProcessor messageProcessor,
        ILogger<ServiceBusMessageHandler> logger,
        Action? criticalExceptionHandler = null)
    {
        _messageProcessor = messageProcessor;
        _logger = logger;

        _criticalExceptionHandler = criticalExceptionHandler ?? _defaultCriticalExceptionHandler;

    }

    /// <summary>
    /// Handles incoming messages from the Service Bus.
    /// </summary>
    /// <param name="args">The arguments containing the message and cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task MessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            await _messageProcessor.ProcessMessageAsync(args.Message, args.CancellationToken);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "error processing message");
            await args.AbandonMessageAsync(args.Message);
        }
    }

    /// <summary>
    /// Handles errors that occur during message processing.
    /// </summary>
    /// <param name="args">The arguments containing the exception and other context information.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ErrorHandler(ProcessErrorEventArgs args)
    {
        if (args.Exception is ServiceBusException { Reason: ServiceBusFailureReason.MessagingEntityNotFound } sbException)
        {
            _logger.LogCritical(sbException, "critical exception occurred while processing message");

            _criticalExceptionHandler();
        }

        _logger.LogError(args.Exception, "error processing message");
        return Task.CompletedTask;
    }
}
