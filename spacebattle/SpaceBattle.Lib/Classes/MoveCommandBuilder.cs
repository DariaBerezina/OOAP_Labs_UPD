using Hwdtech;

namespace SpaceBattle.Lib;

public class MoveCommandBuilder : ICommandBuilder
{
    public ICommand Build(IUObject order, Dictionary<string, object> properties)
    {
        var velocityAdapter = IoC.Resolve<IVelocityChangable>("Adapter.VelocityChangable", order);
        var newVelocity = (Vector)properties["velocity"];
        IoC.Resolve<ICommand>("SetVelocityCommand", velocityAdapter, newVelocity).Execute();
        var movableAdapter = IoC.Resolve<IMovable>("Adapter.Movable", order);
        var moveCommand = IoC.Resolve<ICommand>("MoveCommand", movableAdapter);
        return moveCommand;
    }
}
