using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OnTime.Bus;

public abstract class BusConsumer<T> : BackgroundService where T : IBusMessage
{
    private readonly ChannelReader<T> reader;
    protected readonly ILogger Logger;

    protected BusConsumer(Channel<T> channel, ILogger logger)
    {
        this.reader = channel.Reader;
        this.Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.Logger.LogInformation("Starting consumer for message type {MessageType}...", typeof(T).Name);

        await OnStartedAsync(stoppingToken);

        try
        {
            while (await this.reader.WaitToReadAsync(stoppingToken))
            {
                var message = await this.reader.ReadAsync(stoppingToken);
                try
                {
                    await Consume(message);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "Error occurred while consuming message of type {MessageType}.", typeof(T).Name);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown on cancellation
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Fatal error in consumer dispatch loop for type {MessageType}.", typeof(T).Name);
        }
    }

    protected abstract Task Consume(T message);

    protected virtual Task OnStartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}