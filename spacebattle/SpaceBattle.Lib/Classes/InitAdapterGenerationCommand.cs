using System.Collections.Concurrent;
using Hwdtech;

namespace SpaceBattle.Lib;

public class InitAdapterGenerationCommand : ICommand
{
    public void Execute()
    {
        var generatedTypeCache = new ConcurrentDictionary<Type, Type>();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapter", (object[] args) =>
        {
            var targetInterface = (Type)args[0];
            var targetObj = (IUObject)args[1];

            var adapterType = generatedTypeCache.GetOrAdd(targetInterface,
            (interf) =>
            {
                var code = AdapterCodeGenerator.Generate(interf);
                var baseName = interf.Name.StartsWith('I') ? interf.Name.Substring(1) : interf.Name;
                var className = $"{baseName}Adapter";

                return AdapterCompiler.Compile(code, className);
            });

            return Activator.CreateInstance(adapterType, targetObj)!;
        }).Execute();
    }
}
