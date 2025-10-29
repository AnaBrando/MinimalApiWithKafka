using System.Collections.Concurrent;

namespace KafkaMinimalApi.Infra;


public sealed class BoundedConcurrentQueueEventLog<T> : IEventLog<T>
{
    private readonly int _capacity;
    private readonly ConcurrentQueue<T> _queue = new();

    public BoundedConcurrentQueueEventLog(int capacity = 1000)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        _capacity = capacity;
    }

    public int Count => _queue.Count;

    public void Add(T item)
    {
        _queue.Enqueue(item);
        // Trim without locks; at-most trimming
        while (_queue.Count > _capacity && _queue.TryDequeue(out _)) { }
    }

    public IReadOnlyList<T> Snapshot() => _queue.ToArray();
}