using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class VelocityChangableAdapterTests
{
    private readonly Mock<IUObject> _uObjectMock;
    private readonly Mock<ICommand> _setCommandMock;
    private readonly Vector _testVelocity;

    public VelocityChangableAdapterTests()
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
        _testVelocity = new Vector(-7, 3);


        IoC.Resolve<Hwdtech.ICommand>
        (
            "IoC.Register",
            "VelocityChangable.Velocity.Get",
            (object[] args) =>
            {
                var obj = (IUObject)args[0];
                return obj.GetProperty("velocity");
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>
(
    "IoC.Register",
    "VelocityChangable.Velocity.Set",
    (object[] args) =>
    {
        return _setCommandMock.Object;
    }
).Execute();
    }

    [Fact]
    public void GetVelocity_ReturnsCorrectValue()
    {
        _uObjectMock.Setup(o => o.GetProperty("velocity")).Returns(_testVelocity);
        var adapter = new VelocityChangableAdapter(_uObjectMock.Object);

        var velocity = adapter.Velocity;

        Assert.Equal(_testVelocity, velocity);
        _uObjectMock.Verify(o => o.GetProperty("velocity"), Times.Once);
    }

    [Fact]
    public void SetVelocity_ViaSetCommand_Correctly()
    {
        var adapter = new VelocityChangableAdapter(_uObjectMock.Object);

        adapter.Velocity = _testVelocity;

        _setCommandMock.Verify(c => c.Execute(), Times.Once);
    }

    [Fact]
    public void GetVelocity_MissingProperty_ThrowsException()
    {
        _uObjectMock.Setup(o => o.GetProperty("velocity")).Throws<KeyNotFoundException>();
        var adapter = new VelocityChangableAdapter(_uObjectMock.Object);

        Assert.Throws<KeyNotFoundException>(() => adapter.Velocity);
    }
}
