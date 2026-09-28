using WtLab2_2oop_Ivanov.Models;

namespace WtLab2_2oop_Ivanov.Interfaces;

/// <summary>
/// Дженерик-интерфейс сервиса управления сущностями типа T.
/// T — любой наследник BaseEntity.
/// </summary>
public interface IItemService<T> where T : BaseEntity
{
    void Add(T item);
    T? GetById(int id);
    IEnumerable<T> GetAll();
    bool Remove(int id);
    int Count { get; }

    IEnumerable<T> Find(Func<T, bool> predicate);
    IEnumerable<T> GetAllSorted(Func<T, object> keySelector);
}