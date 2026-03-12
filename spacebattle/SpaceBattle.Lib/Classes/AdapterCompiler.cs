using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SpaceBattle.Lib;

public static class AdapterCompiler
{
    public static Type Compile(string sourceCode, string className)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        MetadataReference[] references = [
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IUObject).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Hwdtech.IoC).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(SpaceBattle.Lib.ICommand).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)
        ];

        var compilation = CSharpCompilation.Create(
            $"Generated_{className}_{Guid.NewGuid():N}",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            throw new InvalidOperationException($"Ошибка компиляции адаптера:\n{string.Join("\n", failures.Select(f => f.GetMessage()))}");
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());

        var type = assembly.GetType($"SpaceBattle.Lib.Generated.{className}");
        return type ?? throw new InvalidOperationException($"Тип {className} не найден в сгенерированной сборке.");
    }
}
