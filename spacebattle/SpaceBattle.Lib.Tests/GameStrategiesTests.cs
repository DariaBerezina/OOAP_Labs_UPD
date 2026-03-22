using System.Reflection;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests;

[Collection("IoC Collection")]
public class GameStrategiesTests
{
    public GameStrategiesTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
    }

    [Fact]
    public void CreateGameStrategy_CreatesScope_And_ReturnsGameCommand()
    {
        var rootScope = IoC.Resolve<object>("Scopes.Current");
        var expectedTimeQuantum = TimeSpan.FromMilliseconds(100);

        var result = CreateGameStrategy.Resolve([expectedTimeQuantum]);

        Assert.IsType<GameCommand>(result);

        var currentScope = IoC.Resolve<object>("Scopes.Current");
        Assert.Same(rootScope, currentScope);

        var gameScopeField = typeof(GameCommand).GetField("_gameScope", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(gameScopeField);

        var gameScope = gameScopeField.GetValue(result);
        Assert.NotNull(gameScope);

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();

        var registeredQuantum = IoC.Resolve<TimeSpan>("Game.TimeQuantum");
        var queue = IoC.Resolve<IQueue>("Game.Queue");
        var repository = IoC.Resolve<IGameObjectRepository>("Game.Repository");

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", rootScope).Execute();

        Assert.Equal(expectedTimeQuantum, registeredQuantum);
        Assert.NotNull(queue);
        Assert.NotNull(repository);
    }

    [Fact]
    public void DeleteGameStrategy_ReturnsCommand_That_ClearsRepository_And_RestoresScope()
    {
        var rootScope = IoC.Resolve<object>("Scopes.Current");
        var gameScope = IoC.Resolve<object>("Scopes.New", rootScope);

        var mockRepo = new Mock<IGameObjectRepository>();
        var mockObject1 = new Mock<IUObject>();
        var mockObject2 = new Mock<IUObject>();

        var objectsInRepo = new Dictionary<string, IUObject>
        {
            { "obj-1", mockObject1.Object },
            { "obj-2", mockObject2.Object }
        };

        mockRepo.Setup(r => r.GetAll()).Returns(objectsInRepo);

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", gameScope).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Repository", (object[] args) => mockRepo.Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", rootScope).Execute();

        var args = new object[] { gameScope };

        var deleteCommand = (Hwdtech.ICommand)DeleteGameStrategy.Resolve(args);

        Assert.IsType<ActionCommand>(deleteCommand);

        deleteCommand.Execute();

        mockRepo.Verify(r => r.Remove("obj-1"), Times.Once);
        mockRepo.Verify(r => r.Remove("obj-2"), Times.Once);

        var currentScope = IoC.Resolve<object>("Scopes.Current");
        Assert.Same(rootScope, currentScope);
    }

    [Fact]
    public void InitGameStrategiesCommand_Registers_Strategies_In_IoC()
    {
        var initCommand = new InitGameStrategiesCommand();
        initCommand.Execute();

        var createStrategy = IoC.Resolve<Func<object[], object>>("Game.CreateNew");
        var deleteStrategy = IoC.Resolve<Func<object[], object>>("Game.Delete");

        Assert.NotNull(createStrategy);
        Assert.NotNull(deleteStrategy);

        var dummyScope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        var createdGameCmd = createStrategy([TimeSpan.FromSeconds(1)]);

        Assert.IsType<GameCommand>(createdGameCmd);

        var deletedGameCmd = deleteStrategy([dummyScope]);
        Assert.IsType<ActionCommand>(deletedGameCmd);
    }
}
