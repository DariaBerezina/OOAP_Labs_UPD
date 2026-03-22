using Hwdtech;
namespace SpaceBattle.Lib;

public class InitGameStrategiesCommand : ICommand
{
    private const string IocRegister = "IoC.Register";
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>(
            IocRegister,
            "Game.CreateNew",
            (Func<object[], object>)CreateGameStrategy.Resolve
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            IocRegister,
            "Game.Delete",
            (Func<object[], object>)DeleteGameStrategy.Resolve
        ).Execute();
    }
}
