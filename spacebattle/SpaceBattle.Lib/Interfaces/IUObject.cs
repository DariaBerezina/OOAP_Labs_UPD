namespace SpaceBattle.Lib;

public interface IUObject
{
    object GetProperty(String key);
    void SetProperty(String key, object value);
    void DeleteProperty(String key);
}
