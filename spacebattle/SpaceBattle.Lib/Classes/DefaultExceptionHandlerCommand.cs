namespace SpaceBattle.Lib;

public class DefaultExceptionHandlerCommand : ICommand
{
    private readonly Exception _exception;

    public DefaultExceptionHandlerCommand(Exception exception)
    {
        _exception = exception;
    }

    public void Execute()
    {
        System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(_exception).Throw();
    }
}
