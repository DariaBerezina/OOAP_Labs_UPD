namespace SpaceBattle.Lib;

public interface IGameObjectRepository
{
    void Add(string id, IUObject uObject);
    IUObject Get(string id);
    void Remove(string id);
    IEnumerable<KeyValuePair<string, IUObject>> GetAll();
}
