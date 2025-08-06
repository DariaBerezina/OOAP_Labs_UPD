using System.Security.Cryptography.X509Certificates;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class StartCommandTests
{
    private readonly Mock<ICommandStartable> _commandStartableMock;
    private readonly Mock<IUObject> _orderMock;
    private readonly Dictionary<string, object> _properties;
    private readonly Mock<IQueue> _queueMock;
    private readonly Mock<ICommandBuilder> _commandBuilderMock;
    private readonly Mock<ICommand> _builtCommand;
    public StartCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>(
            "Scopes.Current.Set",
            IoC.Resolve<object>(
                "Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        _commandStartableMock = new Mock<ICommandStartable>();
        _orderMock = new Mock<IUObject>();
        _properties = new Dictionary<string, object> { ["action"] = "testAction" };
        _queueMock = new Mock<IQueue>();
        _commandBuilderMock = new Mock<ICommandBuilder>();
        _builtCommand = new Mock<ICommand>();

        _commandStartableMock.SetupGet(s => s.Order).Returns(_orderMock.Object);
        _commandStartableMock.SetupGet(s => s.Properties).Returns(_properties);
        _commandStartableMock.SetupGet(s => s.Queue).Returns(_queueMock.Object);

        IoC.Resolve<Hwdtech.ICommand>(
            "IoC.Register",
            "Command.Build.testAction",
            (object[] args) => _commandBuilderMock.Object

        ).Execute();
    }

    [Fact]
    public void Execute_AddCommandToTheQueue_Successfully()
    {
        _commandBuilderMock.Setup(b => b.Build(_orderMock.Object, _properties)).Returns(_builtCommand.Object);
        var command = new StartCommand(_commandStartableMock.Object);

        command.Execute();

        _queueMock.Verify(q => q.Add(_builtCommand.Object), Times.Once);
    }

    [Fact]
    public void Execute_MissingActionProperty_ThrowsException()
    {
        var invalidProperties = new Dictionary<string, object>();
        _commandStartableMock.SetupGet(s => s.Properties).Returns(invalidProperties);

        var command = new StartCommand(_commandStartableMock.Object);

        Assert.Throws<KeyNotFoundException>(command.Execute);
    }

    [Fact]
    public void Execute_Builder_ThrowsException()
    {
        _commandBuilderMock.Setup(b => b.Build(
            It.IsAny<IUObject>(),
            It.IsAny<Dictionary<string, object>>()
            )).Throws<InvalidOperationException>();

        var command = new StartCommand(_commandStartableMock.Object);

        Assert.Throws<InvalidOperationException>(command.Execute);
    }

    [Fact]
    public void Execute_AddCommandToTheQueue_Fails()
    {
        _commandBuilderMock.Setup(b => b.Build(It.IsAny<IUObject>(), It.IsAny<Dictionary<string, object>>())).Returns(_builtCommand.Object);
        _queueMock.Setup(q => q.Add(It.IsAny<ICommand>())).Throws<Exception>();

        var command = new StartCommand(_commandStartableMock.Object);

        Assert.Throws<Exception>(command.Execute);
    }
}
