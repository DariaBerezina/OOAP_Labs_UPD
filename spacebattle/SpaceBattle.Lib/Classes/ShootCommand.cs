using Hwdtech;

namespace SpaceBattle.Lib;

public class ShootCommand : ICommand
{
    private readonly IShootable _shooter;

    public ShootCommand(IShootable shooter)
    {
        _shooter = shooter;
    }

    public void Execute()
    {
        var torpedoPosition = _shooter.Position;
        var torpedoVelocity = IoC.Resolve<Vector>("Game.Math.CalculateTorpedoVelocity", _shooter.Velocity);
        var torpedoId = IoC.Resolve<string>("Game.Id.Create");
        var torpedo = IoC.Resolve<IUObject>("Game.Create.Torpedo", torpedoPosition, torpedoVelocity);
        var repository = IoC.Resolve<IGameObjectRepository>("Game.Repository");
        repository.Add(torpedoId, torpedo);

        var moveCommand = IoC.Resolve<ICommand>("Game.Commands.Move", torpedo);
        var queue = IoC.Resolve<IQueue>("Game.Queue");

        queue.Add(moveCommand);
    }
}
