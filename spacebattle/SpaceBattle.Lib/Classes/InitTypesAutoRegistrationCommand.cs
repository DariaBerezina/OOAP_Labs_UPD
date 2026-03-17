using System.Reflection;
using Hwdtech;

namespace SpaceBattle.Lib;

public class InitTypesAutoRegistrationCommand : ICommand
{
    private readonly Type _targetInterface;
    private readonly string _prefix;
    private readonly string _suffixToRemove;

    public InitTypesAutoRegistrationCommand(Type targetInterface, string prefix, string suffixToRemove)
    {
        _targetInterface = targetInterface;
        _prefix = prefix;
        _suffixToRemove = suffixToRemove;
    }
#pragma warning disable S2325
    public void Execute()
    {
        Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => _targetInterface.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList()
            .ForEach(type =>
            {
                var name = type.Name.Replace(_suffixToRemove, "");

                IoC.Resolve<Hwdtech.ICommand>("IoC.Register", $"{_prefix}{name}", (object[] args) =>
                {
                    return IoC.Resolve<object>("IoC.CreateInstance", type, (IUObject)args[0]);
                }).Execute();
            });
    }
#pragma warning disable S2325
}
