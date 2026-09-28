using WtLab2_2oop_Ivanov.Interfaces;

namespace WtLab2_2oop_Ivanov.Models;

public class Dish : BaseEntity, IStorable
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int CookingTimeMinutes { get; set; }

    // private set — снаружи только через UpdatePrice
    public decimal Price { get; private set; }

    public Dish(int id, string name, decimal price, string category, int cookingTimeMinutes)
    {
        Id = id;
        Name = name;
        Category = category;
        CookingTimeMinutes = cookingTimeMinutes;
        UpdatePrice(price);

        SetInternalTag($"DISH-{Id:D4}");
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Цена должна быть больше нуля.", nameof(newPrice));
        if (newPrice > 100_000m)
            throw new ArgumentException("Цена слишком высокая (макс. 100 000).", nameof(newPrice));

        Price = newPrice;
    }

    public override void PrintInfo()
    {
        Console.WriteLine($"Блюдо №{Id}: «{Name}»");
        Console.WriteLine($"  Категория: {Category}");
        Console.WriteLine($"  Цена: {Price:C}");
        Console.WriteLine($"  Время приготовления: {CookingTimeMinutes} мин.");
        Console.WriteLine($"  Внутренний тег: {GetInternalTag()}");
        Console.WriteLine($"  Создано: {CreatedAt:dd.MM.yyyy HH:mm}");
    }

    public string StorageKey => $"dish:{Id}";
    public void Save() => Console.WriteLine($"[Dish.Save] Сохранено в хранилище: {StorageKey}");
    public string GetDescription() => $"{Name} — {Price:C}";

    public override string GetShortDescription()
        => $"[Блюдо {Id}] {Name} ({Price:C})";
}