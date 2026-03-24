using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class ShootCommandBuilderTests
{
    public ShootCommandBuilderTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
    }

    [Fact]
    public void Build_ShouldReturnShootCommand_WithCorrectAdapter()
    {
        var orderMock = new Mock<IUObject>();
        var properties = new Dictionary<string, object>
        {
            { "action", "Shoot" }
        };

        var shootableAdapterMock = new Mock<IShootable>();
        var expectedCommandMock = new Mock<ICommand>();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapter", (object[] args) =>
        {
            var type = (Type)args[0];
            var obj = (IUObject)args[1];

            if (type == typeof(IShootable) && obj == orderMock.Object)
            {
                return shootableAdapterMock.Object;
            }
            throw new Exception("Wrong adapter configuration");
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.Shoot", (object[] args) =>
        {
            var shooter = (IShootable)args[0];
            if (shooter == shootableAdapterMock.Object)
            {
                return expectedCommandMock.Object;
            }
            throw new Exception("Wrong adapter passed to ShootCommand");
        }).Execute();

        var builder = new ShootCommandBuilder();

        var resultCommand = builder.Build(orderMock.Object, properties);

        Assert.NotNull(resultCommand);
        Assert.Equal(expectedCommandMock.Object, resultCommand);
    }
}
