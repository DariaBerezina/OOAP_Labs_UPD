using System.Diagnostics;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class GameCommandTests
{
    public GameCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>(
                "Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();
    }

    [Fact]
    public void Execute_ShouldSetCorrectScope_AndResolveDependencies()
    {
        var currentTestScope = IoC.Resolve<object>("Scopes.Current");
        var gameScope = IoC.Resolve<object>("Scopes.New", currentTestScope);
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();

        var isQueueCalled = false;
        var queueMock = new Mock<IQueue>();

        queueMock.Setup(q => q.Take()).Returns(new ActionCommand(() => { isQueueCalled = true; }));

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => (object)queueMock.Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.TimeQuantum", (object[] args) => (object)TimeSpan.FromMilliseconds(5)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", currentTestScope).Execute();

        var gameCommand = new GameCommand(gameScope);
        var exception = Record.Exception(() => gameCommand.Execute());

        Assert.Null(exception);
        Assert.True(isQueueCalled);
    }

    [Fact]
    public void Execute_ShouldStop_WhenTimeQuantumIsExceeded()
    {
        var currentTestScope = IoC.Resolve<object>("Scopes.Current");
        var gameScope = IoC.Resolve<object>("Scopes.New", currentTestScope);
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();
        var timeQuantum = TimeSpan.FromMilliseconds(100);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.TimeQuantum", (object[] args) => (object)timeQuantum).Execute();
        var executedCommandsCount = 0;
        var slowCommand = new ActionCommand(() =>
        {
            executedCommandsCount++;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 40)
            {
                Thread.Yield();
            }
        });
        var queueMock = new Mock<IQueue>();
        queueMock.Setup(q => q.Take()).Returns(slowCommand);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => (object)queueMock.Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", currentTestScope).Execute();
        var gameCommand = new GameCommand(gameScope);
        var testStopwatch = Stopwatch.StartNew();

        gameCommand.Execute();
        testStopwatch.Stop();

        Assert.Equal(3, executedCommandsCount);
        Assert.True(testStopwatch.Elapsed >= timeQuantum);
    }

    [Fact]
    public void Execute_ShouldCatchException_AndPassToExceptionHandler()
    {
        var currentTestScope = IoC.Resolve<object>("Scopes.Current");
        var gameScope = IoC.Resolve<object>("Scopes.New", currentTestScope);
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.TimeQuantum", (object[] args) => (object)TimeSpan.FromMilliseconds(50)).Execute();

        var testException = new Exception("Test Exception");
        var faultCommandMock = new Mock<ICommand>();
        faultCommandMock.Setup(c => c.Execute()).Throws(testException);
        var queueMock = new Mock<IQueue>();
        queueMock.Setup(q => q.Take()).Returns(faultCommandMock.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => (object)queueMock.Object).Execute();
        var isHandlerCalled = false;
        var exceptionHandlerMock = new Mock<ICommand>();
        exceptionHandlerMock.Setup(h => h.Execute()).Callback(() => isHandlerCalled = true);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Handler", (object[] args) =>
        {
            var cmd = (ICommand)args[0];
            var ex = (Exception)args[1];

            Assert.Equal(faultCommandMock.Object, cmd);
            Assert.Equal(testException, ex);

            return (object)exceptionHandlerMock.Object;
        }).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", currentTestScope).Execute();
        var gameCommand = new GameCommand(gameScope);

        gameCommand.Execute();

        Assert.True(isHandlerCalled, "Exception Handler не был вызван.");
    }

    [Fact]
    public void DefaultExceptionHandler_ShouldThrowException()
    {
        var exception = new InvalidOperationException("Fatal Error");
        var defaultHandler = new DefaultExceptionHandlerCommand(exception);

        var thrownException = Assert.Throws<InvalidOperationException>(() => defaultHandler.Execute());
        Assert.Equal("Fatal Error", thrownException.Message);
    }
}
