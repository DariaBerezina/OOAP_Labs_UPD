namespace SpaceBattle.Lib;

public interface ICommandBuilder
{
    ICommand Build(ICommandStartable commandStartable);
}
