namespace SpaceBattle.Lib;

public class DictionaryUObject : IUObject
{
    private readonly Dictionary<string, object> _properties = new();

    public object GetProperty(string key) => _properties[key];
    public void SetProperty(string key, object value) => _properties[key] = value;
    public void DeleteProperty(string key) => _properties.Remove(key);
}
