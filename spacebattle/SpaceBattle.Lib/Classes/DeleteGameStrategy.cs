using Hwdtech;

namespace SpaceBattle.Lib;

public static class DeleteGameStrategy
{
    private const string ScopesCurrentSet = "Scopes.Current.Set";

    public static object Resolve(object[] args)
    {
        var gameScope = args[0];

        return new ActionCommand(() =>
        {
            var currentScope = IoC.Resolve<object>("Scopes.Current");
            IoC.Resolve<Hwdtech.ICommand>(ScopesCurrentSet, gameScope).Execute();
            var repository = IoC.Resolve<IGameObjectRepository>("Game.Repository");
            repository.GetAll().Select(x => x.Key).ToList().ForEach(id => repository.Remove(id));
            IoC.Resolve<Hwdtech.ICommand>(ScopesCurrentSet, currentScope).Execute();
        });
    }
}
