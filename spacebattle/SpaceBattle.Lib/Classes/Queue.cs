namespace SpaceBattle.Lib;

public class GameQueue : IQueue
{
    private readonly Queue<ICommand> _queue = new();

    public void Add(ICommand cmd) => _queue.Enqueue(cmd);
    public ICommand Take() => _queue.Count > 0 ? _queue.Dequeue() : new ActionCommand(() => { });
}
