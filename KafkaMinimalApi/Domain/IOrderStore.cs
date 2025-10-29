namespace KafkaMinimalApi.Domain;

public interface IOrderStore
{
    bool TryGet(Guid orderId, out OrderReadModel order);
    IReadOnlyCollection<OrderReadModel> GetAll();
    void Upsert(OrderReadModel order);
    bool TryRemove(Guid orderId);
    int Count { get; }
}

public sealed record OrderReadModel(
    Guid OrderId,
    string CustomerId,
    decimal Amount,
    string Status,
    DateTimeOffset UpdatedAt
);