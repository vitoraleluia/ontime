namespace OnTime.Bus;

public interface IBusProducer<in T> where T : IBusMessage
{
    ValueTask Publish(T message, CancellationToken cancellationToken = default);
}