using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class SetVelocityCommandTests
{
    private readonly Mock<IVelocityChangable> _velocityChangableMock;
    private readonly Vector _testVelocity;
    public SetVelocityCommandTests()
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

        _velocityChangableMock = new Mock<IVelocityChangable>();
        _testVelocity = new Vector(12, 5);
    }

    [Fact]
    public void Execute_SetVelocity_Correctly()
    {
        // Arrange
        var command = new SetVelocityCommand(
            _velocityChangableMock.Object,
            _testVelocity
        );

        command.Execute();

        _velocityChangableMock.VerifySet(
            v => v.Velocity = _testVelocity,
            Times.Once
        );
    }

    [Fact]
    public void Execute_SetVelocity_ThrowsException()
    {
        _velocityChangableMock.SetupSet(c => c.Velocity = It.IsAny<Vector>()).Throws(new InvalidOperationException());

        var command = new SetVelocityCommand(_velocityChangableMock.Object, _testVelocity);

        Assert.Throws<InvalidOperationException>(command.Execute);
    }
}
