namespace SpaceBattle.Lib;

public interface ICommandBuilder
{
    ICommand Build(IUObject order, Dictionary<string, object> properties);
}
