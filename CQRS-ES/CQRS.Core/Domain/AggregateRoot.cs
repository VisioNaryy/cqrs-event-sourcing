using CQRS.Core.Commands;
using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    private readonly List<BaseEvent> _changes = new();

    protected Guid _id;

    public int Version { get; set; } = -1;

    public Guid Id
        => _id;

    public IEnumerable<BaseEvent> GetUncommittedChanges()
        => _changes;

    public void MarkChangesAsCommitted()
        => _changes.Clear();

    private void ApplyChanges(BaseEvent @event, bool isNew)
    {
        // We will get a type of a concrete Aggregate Root
        var method = this.GetType().GetMethod("Apply", new Type[] {@event.GetType()});

        if (method is null)
            throw new ArgumentNullException(nameof(method),
                @$"The \""Apply\"" method was not found in the aggregate {@event.GetType().Name}");

        method.Invoke(this, new object?[] {@event});

        if (isNew is true)
            _changes.Add(@event);
    }

    protected void RaiseEvent(BaseEvent @event)
        => ApplyChanges(@event, true);

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyChanges(@event, false);
        }
    }
}