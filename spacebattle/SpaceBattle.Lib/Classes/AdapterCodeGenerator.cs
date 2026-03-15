using System.Reflection;
using System.Text;

namespace SpaceBattle.Lib;

public static class AdapterCodeGenerator
{
    public static string Generate(Type interfaceType)
    {
        var sb = new StringBuilder();
        var interfaceName = interfaceType.Name;

        var baseName = interfaceName.StartsWith('I') ? interfaceName.Substring(1) : interfaceName;
        var className = $"{baseName}Adapter";

        sb.AppendLine("using System;");
        sb.AppendLine("using SpaceBattle.Lib;");
        sb.AppendLine("using Hwdtech;");
        sb.AppendLine("");
        sb.AppendLine("namespace SpaceBattle.Lib.Generated");
        sb.AppendLine("{");
        sb.AppendLine($"    public class {className} : {interfaceName}");
        sb.AppendLine("    {");

        sb.AppendLine("        private readonly IUObject _target;");
        sb.AppendLine("");

        sb.AppendLine($"        public {className}(IUObject target)");
        sb.AppendLine("        {");
        sb.AppendLine("            _target = target;");
        sb.AppendLine("        }");

        foreach (var prop in interfaceType.GetProperties())
        {
            var propName = prop.Name;
            var propType = prop.PropertyType.Name;

            var attr = interfaceType.GetCustomAttributes<AdapterAttribute>()
                        .FirstOrDefault(a => a.PropertyName == propName);

            string getStrategy = attr != null ? attr.StrategyName : $"{baseName}.{propName}.Get";
            string setStrategy = attr != null ? attr.StrategyName : $"{baseName}.{propName}.Set";

            sb.AppendLine($"        public {propType} {propName}");
            sb.AppendLine("        {");

            if (prop.CanRead)
            {
                sb.AppendLine($"            get => IoC.Resolve<{propType}>(\"{getStrategy}\", _target);");
            }
            if (prop.CanWrite)
            {
                sb.AppendLine($"            set => IoC.Resolve<ICommand>(\"{setStrategy}\", _target, value).Execute();");
            }
            sb.AppendLine("        }");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
