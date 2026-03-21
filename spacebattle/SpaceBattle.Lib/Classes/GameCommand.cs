using System.Diagnostics;
using Hwdtech;

namespace SpaceBattle.Lib;

public class GameCommand : ICommand
{
    private readonly object _gameScope;

    public GameCommand(object gameScope)
    {
        _gameScope = gameScope;
    }

    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", _gameScope).Execute();

        var quantum = IoC.Resolve<TimeSpan>("Game.TimeQuantum");
        var queue = IoC.Resolve<IQueue>("Game.Queue");

        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < quantum)
        {
            var command = queue.Take();

            try
            {
                command.Execute();
            }
            catch (Exception ex)
            {
                var handler = IoC.Resolve<ICommand>("Exception.Handler", command, ex);
                handler.Execute();
            }
        }

        stopwatch.Stop();
    }
}
