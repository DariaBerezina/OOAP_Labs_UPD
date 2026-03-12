namespace SpaceBattle.Lib;

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = true)]
public class AdapterAttribute : Attribute
{
    public Type TargetType { get; }
    public string PropertyName { get; }
    public string StrategyName { get; }

    public AdapterAttribute(Type targetType, string propertyName, string strategyName)
    {
        TargetType = targetType;
        PropertyName = propertyName;
        StrategyName = strategyName;
    }
}
