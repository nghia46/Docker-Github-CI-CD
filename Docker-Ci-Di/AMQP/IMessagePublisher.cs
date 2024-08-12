namespace Docker_Ci_Di.AMQP;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, QueueName queueName, CancellationToken cancellationToken);
}