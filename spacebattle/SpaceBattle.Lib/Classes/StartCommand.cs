using Hwdtech;

namespace SpaceBattle.Lib;

public class StartCommand : ICommand
{
    private readonly ICommandStartable _commandStartable;
    public StartCommand(ICommandStartable commandStartable)
    {
        _commandStartable = commandStartable;
    }
    public void Execute()
    {
        var order = _commandStartable.Order;
        var properties = _commandStartable.Properties;
        var actionType = (string)properties["action"];
        var builder = IoC.Resolve<ICommandBuilder>($"Commnad.Build.{actionType}");
        var cmd = builder.Build(order, properties);
        _commandStartable.Queue.Add(cmd);
    }
}
