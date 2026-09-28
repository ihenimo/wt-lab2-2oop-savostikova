namespace WtLab2_2oop_Ivanov.Interfaces;

/// <summary>
/// Контракт: объект можно сохранить в хранилище и получить его описание.
/// </summary>
public interface IStorable
{
    string StorageKey { get; }
    void Save();
    string GetDescription();
}