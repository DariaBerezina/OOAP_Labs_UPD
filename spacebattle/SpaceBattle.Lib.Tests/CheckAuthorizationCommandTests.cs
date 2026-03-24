using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class CheckAuthorizationCommandTests
{
    private readonly Mock<IUObject> _orderMock;
    private readonly Mock<IAuthorizationInfo> _authInfoMock;
    public CheckAuthorizationCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        _orderMock = new Mock<IUObject>();
        _authInfoMock = new Mock<IAuthorizationInfo>();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapter", (object[] args) =>
        {
            var type = (Type)args[0];
            if (type == typeof(IAuthorizationInfo)) return _authInfoMock.Object;
            throw new Exception("Wrong type");
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Security.CheckAccess", (object[] args) =>
        {
            var userId = (string)args[0];
            var gameItemId = (string)args[1];
            return (object)(userId == "user_42" && gameItemId == "ship_99");
        }).Execute();

    }

    [Fact]
    public void Execute_ShouldPass_WhenUserIsAuthorized()
    {
        _authInfoMock.SetupGet(a => a.UserId).Returns("user_42");
        _authInfoMock.SetupGet(a => a.GameItemId).Returns("ship_99");

        var authCommand = new CheckAuthorizationCommand(_orderMock.Object);

        var exception = Record.Exception(() => authCommand.Execute());

        Assert.Null(exception);
    }

    [Fact]
    public void Execute_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
    {
        _authInfoMock.SetupGet(a => a.UserId).Returns("hacker_1337");
        _authInfoMock.SetupGet(a => a.GameItemId).Returns("ship_99");

        var authCommand = new CheckAuthorizationCommand(_orderMock.Object);

        var exception = Assert.Throws<UnauthorizedAccessException>(() => authCommand.Execute());

        Assert.Contains("User hacker_1337 is not authorized", exception.Message);
    }
}
