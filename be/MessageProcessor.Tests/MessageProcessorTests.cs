using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MessageProcessor.Tests;

public class MessageProcessorTests
{
    [Fact]
    public async Task Handler_Should_Process_Message()
    {
        // Arrange
        var mockProcessor = Substitute.For<IMessageProcessor>();
        var handler = new ServiceBusMessageHandler(mockProcessor, Substitute.For<ILogger<ServiceBusMessageHandler>>());
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(body: BinaryData.FromString("Test message"));

        var args = Substitute.For<ProcessMessageEventArgs>(
            message,
            Substitute.For<ServiceBusReceiver>(),
            CancellationToken.None);

        // Act
        await handler.MessageHandler(args);

        // Assert
        await mockProcessor.Received(1).ProcessMessageAsync(message, CancellationToken.None);
        await args.Received(1).CompleteMessageAsync(message);
    }

    [Fact]
    public async Task Handler_Should_Handle_Exceptions()
    {
        // Arrange
        var mockProcessor = Substitute.For<IMessageProcessor>();

        mockProcessor.ProcessMessageAsync(Arg.Any<ServiceBusReceivedMessage>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new Exception("processing error"));

        var handler = new ServiceBusMessageHandler(mockProcessor, Substitute.For<ILogger<ServiceBusMessageHandler>>());
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(body: BinaryData.FromString("Test message"));

        var args = Substitute.For<ProcessMessageEventArgs>(
            message,
            Substitute.For<ServiceBusReceiver>(),
            CancellationToken.None);

        // Act
        await handler.MessageHandler(args);

        // Assert
        await mockProcessor.Received(1).ProcessMessageAsync(message, CancellationToken.None);
        await args.Received(1).AbandonMessageAsync(message);
    }

    [Fact]
    public async Task ErrorHandler_Should_Call_Critical_Exception_Handler_And_Log_Critical_On_ServiceBusException()
    {
        // Arrange
        var mockProcessor = Substitute.For<IMessageProcessor>();
        var logger = Substitute.For<ILogger<ServiceBusMessageHandler>>();
        var criticalExceptionHandlerCalled = false;
        var handler = new ServiceBusMessageHandler(mockProcessor, logger, () => { criticalExceptionHandlerCalled = true; });

        var exception = new ServiceBusException("Entity not found", ServiceBusFailureReason.MessagingEntityNotFound);

        var args = new ProcessErrorEventArgs(
            exception,
            ServiceBusErrorSource.Receive,
            "namespace",
            "entity path",
            "identifier",
            CancellationToken.None);

        // Act
        await handler.ErrorHandler(args);

        // Assert
        criticalExceptionHandlerCalled.Should().BeTrue();
        logger.Received(1).LogCritical(exception, "critical exception occurred while processing message");
    }

    [Fact]
    public async Task ErrorHandler_Should_Not_Call_Critical_Exception_Handler_And_Log_Error_On_Non_ServiceBusException()
    {
        // Arrange
        var mockProcessor = Substitute.For<IMessageProcessor>();
        var logger = Substitute.For<ILogger<ServiceBusMessageHandler>>();
        var criticalExceptionHandlerCalled = false;
        var handler = new ServiceBusMessageHandler(mockProcessor, logger, () => { criticalExceptionHandlerCalled = true; });

        var exception = new Exception("error occurred");

        var args = new ProcessErrorEventArgs(
            exception,
            ServiceBusErrorSource.Receive,
            "namespace",
            "entity path",
            "identifier",
            CancellationToken.None);

        // Act
        await handler.ErrorHandler(args);

        // Assert
        criticalExceptionHandlerCalled.Should().BeFalse();
        logger.Received(1).LogError(exception, "error processing message");
    }
}
