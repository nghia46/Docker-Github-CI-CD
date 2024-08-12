using MassTransit;

namespace Docker_Ci_Di.AMQP;

public class MessagePublisher : IMessagePublisher
{
    private readonly ISendEndpointProvider _provider;

    public MessagePublisher(ISendEndpointProvider provider)
    {
        _provider = provider;
    }

    public async Task PublishAsync<T>(T message, QueueName queueName, CancellationToken cancellationToken)
    {
        var endpoint = await _provider.GetSendEndpoint(new Uri($"queue:{queueName}"));
        if (message != null) await endpoint.Send(message,cancellationToken);
    }
}