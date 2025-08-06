using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class MovableAdapterTests
{
    private readonly Mock<IUObject> _uObjectMock;
    private readonly Mock<ICommand> _setCommandMock;
    private readonly Vector _testPosition;
    private readonly Vector _testVelocity;

    public MovableAdapterTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>
        (
            "Scopes.Current.Set",
            IoC.Resolve<object>(
                "Scopes.New",
                IoC.Resolve<object>("Scopes.Root")
            )
        ).Execute();

        _uObjectMock = new Mock<IUObject>();
        _setCommandMock = new Mock<ICommand>();
        _testPosition = new Vector(12, 5);
        _testVelocity = new Vector(-7, 3);

        IoC.Resolve<Hwdtech.ICommand>
        (
            "IoC.Register",
            "Movable.Position",
            (object[] args) =>
            {
                var obj = (IUObject)args[0];
                return obj.GetProperty("position");
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>
        (
            "IoC.Register",
            "Movable.Position.Set",
            (object[] args) =>
            {
                return _setCommandMock.Object;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>
        (
            "IoC.Register",
            "Movable.Velocity",
            (object[] args) =>
            {
                var obj = (IUObject)args[0];
                return obj.GetProperty("velocity");
            }
        ).Execute();
    }

    [Fact]
    public void GetPosition_ReturnsCorrectValue()
    {
        _uObjectMock.Setup(o => o.GetProperty("position")).Returns(_testPosition);
        var adapter = new MovableAdapter(_uObjectMock.Object);

        var position = adapter.Position;

        Assert.Equal(_testPosition, position);
        _uObjectMock.Verify(o => o.GetProperty("position"), Times.Once);
    }

    [Fact]
    public void SetPosition_ViaSetCommand_Correctly()
    {
        var adapter = new MovableAdapter(_uObjectMock.Object);

        adapter.Position = _testPosition;

        _setCommandMock.Verify(c => c.Execute(), Times.Once);
    }

    [Fact]
    public void GetVelocity_ReturnsCorrectValue()
    {
        _uObjectMock.Setup(o => o.GetProperty("velocity")).Returns(_testVelocity);
        var adapter = new MovableAdapter(_uObjectMock.Object);

        var velocity = adapter.Velocity;

        Assert.Equal(_testVelocity, velocity);
        _uObjectMock.Verify(o => o.GetProperty("velocity"), Times.Once);
    }

    [Fact]
    public void GetPosition_MissingProperty_ThrowsException()
    {
        _uObjectMock.Setup(o => o.GetProperty("position")).Throws<KeyNotFoundException>();
        var adapter = new MovableAdapter(_uObjectMock.Object);

        Assert.Throws<KeyNotFoundException>(() => adapter.Position);
    }

    [Fact]
    public void GetVelocity_MissingProperty_ThrowsException()
    {
        _uObjectMock.Setup(o => o.GetProperty("velocity")).Throws<KeyNotFoundException>();
        var adapter = new MovableAdapter(_uObjectMock.Object);

        Assert.Throws<KeyNotFoundException>(() => adapter.Velocity);
    }
}
