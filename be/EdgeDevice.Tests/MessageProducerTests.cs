using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text.Json;
using Contracts;
using FluentAssertions;

namespace EdgeDevice.Tests;

public class MessageProducerTests
{
    public class MockMessage : IDeviceMessage
    {
        public int Id { get; set; }
        public required string DeviceId { get; set; }
        public string MessageType { get; set; } = "mock";
        public int Version { get; set; } = 1;
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }

    [Fact]
    public async Task SendMessageAsync_Json_Serializes_And_Sends_The_Message()
    {
        // Arrange
        var mockSender = Substitute.For<ServiceBusSender>();
        var mockLogger = Substitute.For<ILogger<MessageProducer<MockMessage>>>();

        var producer = new MessageProducer<MockMessage>(mockSender, mockLogger);
        var cts = new CancellationTokenSource();

        var message = new MockMessage { Id = 100, DeviceId = "device-100" };

        ServiceBusMessage capturedMessage = null!;

        await mockSender.SendMessageAsync(Arg.Do<ServiceBusMessage>(msg => capturedMessage = msg), Arg.Any<CancellationToken>());

        // Act
        await producer.SendMessageAsync(message, CancellationToken.None);

        await cts.CancelAsync();

        // Assert
        await mockSender.Received(1)
            .SendMessageAsync(Arg.Any<ServiceBusMessage>(), Arg.Any<CancellationToken>());

        // Assert the message id was added
        var messageIdValid = Guid.TryParse(capturedMessage.MessageId, out var _);

        messageIdValid.Should().BeTrue();

        // Assert the message sent was JSON serialized
        var messageBody = capturedMessage!.Body.ToArray();
        var deserializedMessage = JsonSerializer.Deserialize<MockMessage>(messageBody);

        deserializedMessage!.Id.Should().Be(message.Id);
        deserializedMessage!.DeviceId.Should().Be(message.DeviceId);
        deserializedMessage!.MessageType.Should().Be(message.MessageType);
        deserializedMessage!.Version.Should().Be(message.Version);
        deserializedMessage!.Timestamp.CompareTo(DateTimeOffset.UtcNow).Should().Be(-1);
    }
}
