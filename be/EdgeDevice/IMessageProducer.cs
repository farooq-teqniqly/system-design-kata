namespace EdgeDevice;
public interface IMessageProducer<in TMessage>
{
    Task SendMessageAsync(TMessage message, CancellationToken ctx=default);
}
