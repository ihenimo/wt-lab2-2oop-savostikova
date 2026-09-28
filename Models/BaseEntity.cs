namespace WtLab2_2oop_Ivanov.Models;

/// <summary>
/// Абстрактный базовый класс для всех сущностей.
/// </summary>
public abstract class BaseEntity
{
    // protected — доступно наследникам, снаружи нельзя
    protected string _internalTag = string.Empty;

    public int Id { get; protected set; }
    public DateTime CreatedAt { get; private set; }

    protected BaseEntity()
    {
        CreatedAt = DateTime.Now;
    }

    public abstract void PrintInfo();

    public virtual string GetShortDescription()
        => $"[{Id}] создано {CreatedAt:dd.MM.yyyy HH:mm}";

    protected void SetInternalTag(string tag)
    {
        _internalTag = tag;
    }

    public string GetInternalTag() => _internalTag;
}