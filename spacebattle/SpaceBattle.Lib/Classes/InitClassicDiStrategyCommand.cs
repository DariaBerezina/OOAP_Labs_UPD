using Hwdtech;

namespace SpaceBattle.Lib;

public class InitClassicDiStrategyCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "IoC.CreateInstance", (object[] args) =>
        {
            var targetType = (Type)args[0];
            var contextArgs = args.Skip(1).ToArray();
            var constructor = targetType.GetConstructors().Single();
            var resolvedArgs = constructor.GetParameters()
                .Select(p => IoC.Resolve<object>(p.ParameterType.Name, contextArgs))
                .ToArray();

            return constructor.Invoke(resolvedArgs);
        }).Execute();
    }
}
