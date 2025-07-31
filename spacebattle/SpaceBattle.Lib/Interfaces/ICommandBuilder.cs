namespace SpaceBattle.Lib;

public interface ICommandBuilder<in T>
{
    ICommand Build(T commandData);
}
