using Hwdtech;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Tests;

public class InitClassicDiStrategyCommandTests
{
    public InitClassicDiStrategyCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set",
            IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))
        ).Execute();
    }

    public interface IDummyDependency { }

    public class DummyDependency : IDummyDependency { }

    public class DummyClass
    {
        public IDummyDependency Dependency { get; }

        public DummyClass(IDummyDependency dependency)
        {
            Dependency = dependency;
        }
    }

    public class MultiConstructorClass
    {
        public IDummyDependency? Dependency { get; }

        public MultiConstructorClass() { }
        public MultiConstructorClass(IDummyDependency dependency)
        {
            Dependency = dependency;
        }
    }

    [Fact]
    public void Execute_ShouldRegisterCreateInstanceStrategy_AndResolveDependencies()
    {
        var command = new InitClassicDiStrategyCommand();
        command.Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "IDummyDependency", (object[] args) =>
        {
            return new DummyDependency();
        }).Execute();

        var result = IoC.Resolve<object>("IoC.CreateInstance", typeof(DummyClass));

        Assert.NotNull(result);
        Assert.IsType<DummyClass>(result);

        var dummyInstance = (DummyClass)result;
        Assert.NotNull(dummyInstance.Dependency);
        Assert.IsType<DummyDependency>(dummyInstance.Dependency);
    }

    [Fact]
    public void Execute_ShouldPassContextArguments_ToDependencies()
    {
        var command = new InitClassicDiStrategyCommand();
        command.Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "IDummyDependency", (object[] args) =>
        {
            var expectedContextString = (string)args[0];

            Assert.Equal("TestContext", expectedContextString);

            return new DummyDependency();
        }).Execute();

        var result = IoC.Resolve<object>("IoC.CreateInstance", typeof(DummyClass), "TestContext");

        Assert.NotNull(result);
        Assert.IsType<DummyClass>(result);
    }

    [Fact]
    public void CreateInstance_ShouldThrowException_IfClassHasMultipleConstructors()
    {
        var command = new InitClassicDiStrategyCommand();
        command.Execute();

        Assert.Throws<InvalidOperationException>(() =>
            IoC.Resolve<object>("IoC.CreateInstance", typeof(MultiConstructorClass))
        );
    }

    [Fact]
    public void CreateInstance_ShouldThrowException_IfDependencyNotRegistered()
    {
        var command = new InitClassicDiStrategyCommand();
        command.Execute();

        Assert.ThrowsAny<Exception>(() =>
            IoC.Resolve<object>("IoC.CreateInstance", typeof(DummyClass))
        );
    }
}
