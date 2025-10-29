namespace KafkaMinimalApi.Infra;

public interface IEventLog<T>
{
    void Add(T item);
    IReadOnlyList<T> Snapshot();
    int Count { get; }
}