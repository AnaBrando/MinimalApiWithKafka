using System.Collections.Concurrent;
using KafkaMinimalApi.Domain;

public sealed class InMemoryOrderStore : IOrderStore
{
    private readonly ConcurrentDictionary<Guid, OrderReadModel> _orders = new();

    public int Count => _orders.Count;

    public bool TryGet(Guid orderId, out OrderReadModel order) =>
        _orders.TryGetValue(orderId, out order!);

    public IReadOnlyCollection<OrderReadModel> GetAll() =>
        // Snapshot to avoid enumeration hazards and present a stable view
        _orders.Values.ToArray();

    public void Upsert(OrderReadModel order) =>
        _orders.AddOrUpdate(order.OrderId, order, (_, __) => order);

    public bool TryRemove(Guid orderId) =>
        _orders.TryRemove(orderId, out _);
}