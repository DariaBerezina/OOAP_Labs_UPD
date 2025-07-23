using Hwdtech;

namespace SpaceBattle.Lib;

public class MoveCommandBuilder : ICommandBuilder
{
    public ICommand Build(ICommandStartable commandStartable)
    {
        var movingObject = commandStartable.Order;
        var velocityAdapter = IoC.Resolve<IVelocityChangable>("Adapter.Velocity", movingObject);
        var newVelocity = (Vector)commandStartable.Properties["velocity"];
        IoC.Resolve<ICommand>("SetVelocityCommand", velocityAdapter, newVelocity).Execute();
        var movableAdpater = IoC.Resolve<IMovable>("Adapter.Movable", movingObject);
        var moveCmd = IoC.Resolve<ICommand>("MoveCommand", movableAdpater);
        return moveCmd;
    }
}
