using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class GameObjectRepository : IGameObjectRepository
{
    private readonly ConcurrentDictionary<string, IUObject> _repository = new();

    public void Add(string id, IUObject uObject)
    {
        if (!_repository.TryAdd(id, uObject))
        {
            throw new ArgumentException($"Object with id {id} already exists in the repository.");
        }
    }

    public IUObject Get(string id)
    {
        if (_repository.TryGetValue(id, out var uObject))
        {
            return uObject;
        }
        throw new KeyNotFoundException($"Object with id {id} not found.");
    }

    public void Remove(string id)
    {
        _repository.TryRemove(id, out _);
    }

    public IEnumerable<KeyValuePair<string, IUObject>> GetAll()
    {
        return _repository.ToArray();
    }
}
