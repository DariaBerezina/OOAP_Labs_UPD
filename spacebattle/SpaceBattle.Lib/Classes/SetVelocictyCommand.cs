namespace SpaceBattle.Lib;

public class SetVelocityCommand : ICommand
{
    private readonly IVelocityChangable _velocityChangable;
    private readonly Vector _newVelocity;
    public SetVelocityCommand(IVelocityChangable velocityChangable, Vector newVelocity)
    {
        _velocityChangable = velocityChangable;
        _newVelocity = newVelocity;
    }
    public void Execute()
    {
        _velocityChangable.Velocity = _newVelocity;
    }
}