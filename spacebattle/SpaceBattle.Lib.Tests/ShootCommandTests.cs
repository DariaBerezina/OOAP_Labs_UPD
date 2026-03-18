using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

public class ShootCommandTests
{

    private readonly Mock<IUObject> _torpedoMock;
    private readonly Mock<IMovable> _movableMock;
    private readonly Mock<ICommand> _moveCommandMock;
    private readonly Mock<IGameObjectRepository> _repoMock;
    private readonly Mock<IQueue> _queueMock;
    public ShootCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        _torpedoMock = new Mock<IUObject>();
        _movableMock = new Mock<IMovable>();
        _moveCommandMock = new Mock<ICommand>();
        _repoMock = new Mock<IGameObjectRepository>();
        _queueMock = new Mock<IQueue>();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Math.CalculateTorpedoVelocity", (object[] args) =>
        {
            return new Vector(4, 4); // Пусть торпеда летит в 2 раза быстрее
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Id.Create", (object[] args) => "torpedo_123").Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Create.Torpedo", (object[] args) => _torpedoMock.Object).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Repository", (object[] args) => _repoMock.Object).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapter", (object[] args) =>
        {
            var type = (Type)args[0];
            if (type == typeof(IMovable)) return _movableMock.Object;
            throw new ArgumentException("Unexpected adapter type");
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.Move", (object[] args) => _moveCommandMock.Object).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => _queueMock.Object).Execute();

    }

    [Fact]
    public void Execute_ShouldCreateTorpedo_AndPutMoveCommandToQueue()
    {
        var shooterMock = new Mock<IShootable>();
        shooterMock.SetupGet(s => s.Position).Returns(new Vector(10, 10));
        shooterMock.SetupGet(s => s.Velocity).Returns(new Vector(2, 2));
        var shootCommand = new ShootCommand(shooterMock.Object);

        shootCommand.Execute();

        _repoMock.Verify(r => r.Add("torpedo_123", _torpedoMock.Object), Times.Once);
        _queueMock.Verify(q => q.Add(_moveCommandMock.Object), Times.Once);
    }
}
