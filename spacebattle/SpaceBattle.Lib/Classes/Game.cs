namespace SpaceBattle.Lib;

public class Game
{
    private readonly IQueue _queue;

    public Game(IQueue queue)
    {
        _queue = queue;
    }

    public void Step()
    {
        var cmd = _queue.Take();
        cmd.Execute();
    }
}
