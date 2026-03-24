using Hwdtech;

namespace SpaceBattle.Lib;

public class InitExceptionHandlerStrategyCommand : ICommand
{
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Exception.Handler",
            (object[] args) => ExceptionHandlerStrategy.Resolve(args)
        ).Execute();
    }
}
