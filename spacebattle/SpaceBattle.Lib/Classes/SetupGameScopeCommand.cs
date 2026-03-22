using Hwdtech;

namespace SpaceBattle.Lib;

public class SetupGameScopeCommand : ICommand
{
    private const string IocRegister = "IoC.Register";
    private readonly TimeSpan _timeQuantum;

    public SetupGameScopeCommand(TimeSpan timeQuantum)
    {
        _timeQuantum = timeQuantum;
    }

    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>(
            IocRegister,
            "Game.TimeQuantum",
            (Func<object[], object>)((object[] a) => _timeQuantum)
        ).Execute();

        var gameQueue = new GameQueue();

        IoC.Resolve<Hwdtech.ICommand>(
            IocRegister,
            "Game.Queue",
            (Func<object[], object>)((object[] a) => gameQueue)
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            IocRegister,
            "Game.Queue.Push",
            (Func<object[], object>)((object[] a) => new ActionCommand(() => gameQueue.Add((ICommand)a[0])))
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            IocRegister,
            "Game.Queue.Take",
            (Func<object[], object>)((object[] a) => gameQueue.Take())
        ).Execute();

        var repository = new GameObjectRepository();

        IoC.Resolve<Hwdtech.ICommand>(
            IocRegister,
            "Game.Repository",
            (Func<object[], object>)((object[] a) => repository)
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            IocRegister,
            "Game.Objects.Get",
            (Func<object[], object>)((object[] a) => repository.Get((string)a[0]))
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            IocRegister,
            "Game.Objects.Delete",
            (Func<object[], object>)((object[] a) => new ActionCommand(() => repository.Remove((string)a[0])))
        ).Execute();
    }
}
