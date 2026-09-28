using WtLab2_2oop_Ivanov.Interfaces;

namespace WtLab2_2oop_Ivanov.Models;

public class Restaurant : BaseEntity, IStorable
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    private readonly List<Dish> _menu = new();
    public IReadOnlyList<Dish> Menu => _menu;

    public Restaurant(int id, string name, string address)
    {
        Id = id;
        Name = name;
        Address = address;

        SetInternalTag($"RESTAURANT-{Id:D4}");
    }

    public void AddDish(Dish dish)
    {
        if (dish is null)
            throw new ArgumentNullException(nameof(dish));
        if (_menu.Any(d => d.Name == dish.Name))
            throw new InvalidOperationException($"Блюдо «{dish.Name}» уже есть в меню.");
        _menu.Add(dish);
    }

    public override void PrintInfo()
    {
        Console.WriteLine($"Ресторан №{Id}: «{Name}»");
        Console.WriteLine($"  Адрес: {Address}");
        Console.WriteLine($"  Блюд в меню: {_menu.Count}");
        Console.WriteLine($"  Внутренний тег: {GetInternalTag()}");
        Console.WriteLine($"  Создано: {CreatedAt:dd.MM.yyyy HH:mm}");
    }

    public string StorageKey => $"restaurant:{Id}";
    public void Save() => Console.WriteLine($"[Restaurant.Save] Сохранено: {StorageKey}");
    public string GetDescription() => $"{Name} ({Address})";

    public override string GetShortDescription()
        => $"[Ресторан {Id}] {Name} — {_menu.Count} блюд";
}