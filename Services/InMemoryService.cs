using WtLab2_2oop_Ivanov.Interfaces;
using WtLab2_2oop_Ivanov.Models;

namespace WtLab2_2oop_Ivanov.Services;

public class InMemoryService<T> : IItemService<T> where T : BaseEntity
{
    private readonly List<T> _items = new();

    public int Count => _items.Count;

    public void Add(T item)
    {
        if (item is null)
            throw new ArgumentNullException(nameof(item));

        if (_items.Any(x => x.Id == item.Id))
            throw new InvalidOperationException(
                $"Элемент с Id={item.Id} уже существует в сервисе {typeof(T).Name}.");

        _items.Add(item);
    }

    public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    public IEnumerable<T> GetAll() => _items;

    public bool Remove(int id)
    {
        var item = GetById(id);
        if (item is null) return false;
        return _items.Remove(item);
    }

    public IEnumerable<T> Find(Func<T, bool> predicate)
    {
        if (predicate is null)
            throw new ArgumentNullException(nameof(predicate));
        return _items.Where(predicate);
    }

    public IEnumerable<T> GetAllSorted(Func<T, object> keySelector)
    {
        if (keySelector is null)
            throw new ArgumentNullException(nameof(keySelector));
        return _items.OrderBy(keySelector);
    }
}