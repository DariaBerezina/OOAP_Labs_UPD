using Hwdtech;

namespace SpaceBattle.Lib;

public static class CreateGameStrategy
{
    private const string ScopesCurrentSet = "Scopes.Current.Set";

    public static object Resolve(object[] args)
    {
        var timeQuantum = (TimeSpan)args[0];
        var parentScope = IoC.Resolve<object>("Scopes.Current");
        var gameScope = IoC.Resolve<object>("Scopes.New", parentScope);
        IoC.Resolve<Hwdtech.ICommand>(ScopesCurrentSet, gameScope).Execute();
        new SetupGameScopeCommand(timeQuantum).Execute();
        IoC.Resolve<Hwdtech.ICommand>(ScopesCurrentSet, parentScope).Execute();

        return new GameCommand(gameScope);
    }
}
