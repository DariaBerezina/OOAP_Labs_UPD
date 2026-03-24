using Hwdtech;

namespace SpaceBattle.Lib;

public class InitUniversalDiStrategyCommand : ICommand
{
    public void Execute()
    {
        var argumentResolvers = new Dictionary<bool, Func<Type, IUObject, object>>
        {
            {true, (type, uObj) => IoC.Resolve<object>("Adapter", type, uObj)},

            {false, (type, uObj) => IoC.Resolve<object>(type.Name, uObj)}
        };

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "IoC.CreateInstance", (object[] args) =>
        {
            var targetType = (Type)args[0];
            var uObject = (IUObject)args[1];

            var constructor = targetType.GetConstructors().Single();

            var resolvedArgs = constructor.GetParameters()
                .Select(p => argumentResolvers[p.ParameterType.IsInterface](p.ParameterType, uObject))
                .ToArray();

            return constructor.Invoke(resolvedArgs);
        }).Execute();
    }
}
