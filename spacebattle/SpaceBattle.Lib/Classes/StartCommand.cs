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
        var actionType = (string)_commandStartable.Properties["action"];
        var builder = IoC.Resolve<ICommandBuilder>($"Command.Build.{actionType}", _commandStartable);
        var cmd = builder.Build(_commandStartable);
        _commandStartable.Queue.Add(cmd);
    }
}
