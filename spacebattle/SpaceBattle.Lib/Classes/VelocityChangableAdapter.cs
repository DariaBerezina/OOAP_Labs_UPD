using Hwdtech;

namespace SpaceBattle.Lib;

public class VelocityChangableAdapter : IVelocityChangable
{
    private readonly IUObject _uObject;
    public VelocityChangableAdapter(IUObject uObject)
    {
        _uObject = uObject;
    }
    public Vector Velocity
    {
        get => IoC.Resolve<Vector>("VelocityChangable.Velocity.Get", _uObject);
        set => IoC.Resolve<ICommand>("VelocityChangable.Velocity.Set", _uObject, value).Execute();
    }
}
