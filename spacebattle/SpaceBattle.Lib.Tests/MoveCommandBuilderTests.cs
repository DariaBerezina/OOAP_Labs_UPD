using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Tests
{
    public class MoveCommandBuilderTests
    {
        private readonly Mock<IUObject> _orderMock;
        private readonly Dictionary<string, object> _properties;
        private readonly Mock<IVelocityChangable> _velocityAdapterMock;
        private readonly Mock<IMovable> _movableAdapterMock;
        private readonly Mock<ICommand> _setVelocityCommandMock;
        private readonly Mock<ICommand> _moveCommandMock;
        private readonly Vector _testVelocity = new Vector(12, 5);

        public MoveCommandBuilderTests()
        {
            // Инициализация IoC контейнера
            new InitScopeBasedIoCImplementationCommand().Execute();
            IoC.Resolve<Hwdtech.ICommand>(
                "Scopes.Current.Set",
                IoC.Resolve<object>(
                    "Scopes.New",
                    IoC.Resolve<object>("Scopes.Root")
                )
            ).Execute();

            _orderMock = new Mock<IUObject>();
            _properties = new Dictionary<string, object> { ["velocity"] = _testVelocity };
            _velocityAdapterMock = new Mock<IVelocityChangable>();
            _movableAdapterMock = new Mock<IMovable>();
            _setVelocityCommandMock = new Mock<ICommand>();
            _moveCommandMock = new Mock<ICommand>();

            // Регистрация зависимостей
            IoC.Resolve<Hwdtech.ICommand>(
                "IoC.Register",
                "Adapter.VelocityChangable",
                (object[] args) => _velocityAdapterMock.Object
            ).Execute();

            IoC.Resolve<Hwdtech.ICommand>(
                "IoC.Register",
                "SetVelocityCommand",
                (object[] args) => _setVelocityCommandMock.Object
            ).Execute();

            IoC.Resolve<Hwdtech.ICommand>(
                "IoC.Register",
                "Adapter.Movable",
                (object[] args) => _movableAdapterMock.Object
            ).Execute();

            IoC.Resolve<Hwdtech.ICommand>(
                "IoC.Register",
                "MoveCommand",
                (object[] args) => _moveCommandMock.Object
            ).Execute();
        }

        [Fact]
        public void Build_ValidParameters_ReturnsMoveCommand()
        {
            var builder = new MoveCommandBuilder();

            var command = builder.Build(_orderMock.Object, _properties);

            Assert.Same(_moveCommandMock.Object, command);
            _setVelocityCommandMock.Verify(c => c.Execute(), Times.Once);
        }

        [Fact]
        public void Build_SetVelocityCommand_ExecutesCorrectly()
        {
            var builder = new MoveCommandBuilder();

            builder.Build(_orderMock.Object, _properties);

            _setVelocityCommandMock.Verify(s => s.Execute(), Times.Once);
        }

        [Fact]
        public void Build_SetVelocityCommnad_Fails()
        {
            _setVelocityCommandMock.Setup(s => s.Execute()).Throws<InvalidOperationException>();
            var builder = new MoveCommandBuilder();

            Assert.Throws<InvalidOperationException>(() => builder.Build(_orderMock.Object, _properties));
        }

        [Fact]
        public void Build_MoveCommand_NeverExecutes()
        {
            var builder = new MoveCommandBuilder();

            builder.Build(_orderMock.Object, _properties);

            _moveCommandMock.Verify(m => m.Execute(), Times.Never);
        }

        [Fact]
        public void Build_MissingVelocityProperty_ThrowsException()
        {
            var invalidProperties = new Dictionary<string, object>();
            var builder = new MoveCommandBuilder();

            Assert.Throws<KeyNotFoundException>(() => builder.Build(_orderMock.Object, invalidProperties));
        }

    }
}
