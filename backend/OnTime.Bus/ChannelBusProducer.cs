using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OnTime.Bus;

public class ChannelBusProducer<T> : IBusProducer<T> where T : IBusMessage
{
    private readonly ChannelWriter<T> writer;

    public ChannelBusProducer(Channel<T> channel)
    {
        this.writer = channel.Writer;
    }

    public async ValueTask Publish(T message, CancellationToken cancellationToken = default)
    {
        await this.writer.WriteAsync(message, cancellationToken);
    }
}