using Hwdtech;

namespace SpaceBattle.Lib;

public static class ExceptionHandlerStrategy
{
    public static object Resolve(object[] args)
    {
        var command = (ICommand)args[0];
        var exception = (Exception)args[1];
        var cmdTypeName = command.GetType().Name;
        var exTypeName = exception.GetType().Name;
        var specificHandlerKey = $"Exception.Handler.{cmdTypeName}.{exTypeName}";
        try
        {
            return IoC.Resolve<ICommand>(specificHandlerKey, command, exception);
        }
        catch
        {
            return new DefaultExceptionHandlerCommand(exception);
        }
    }
}
