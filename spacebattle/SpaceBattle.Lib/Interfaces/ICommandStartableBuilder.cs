namespace SpaceBattle.Lib;

public interface ICommandStartableBuilder
{
    ICommand Build(ICommandStartable commandStartable);
}
