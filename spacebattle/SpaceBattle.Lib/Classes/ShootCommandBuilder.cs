using Hwdtech;

namespace SpaceBattle.Lib;

public class ShootCommandBuilder : ICommandBuilder
{
    public ICommand Build(IUObject order, Dictionary<string, object> properties)
    {
        var shooterAdapter = IoC.Resolve<IShootable>("Adapter", typeof(IShootable), order);

        return IoC.Resolve<ICommand>("Game.Commands.Shoot", shooterAdapter);
    }
}
