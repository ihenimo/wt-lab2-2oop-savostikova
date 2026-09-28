using WtLab2_2oop_Ivanov.Interfaces;

namespace WtLab2_2oop_Ivanov.Models;

public class Courier : BaseEntity, IStorable
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;

    // private set — через Rate()
    public double Rating { get; private set; }

    public Courier(int id, string fullName, string phone, double rating = 5.0)
    {
        Id = id;
        FullName = fullName;
        Phone = phone;
        Rating = 5.0;
        Rate(rating);

        SetInternalTag($"COURIER-{Id:D4}");
    }

    public void Rate(double newRating)
    {
        if (newRating < 0 || newRating > 5)
            throw new ArgumentOutOfRangeException(nameof(newRating), "Рейтинг должен быть от 0 до 5.");
        Rating = newRating;
    }

    public override void PrintInfo()
    {
        Console.WriteLine($"Курьер №{Id}: {FullName}");
        Console.WriteLine($"  Телефон: {Phone}");
        Console.WriteLine($"  Рейтинг: {Rating:F1}");
        Console.WriteLine($"  Статус: {(IsAvailable ? "свободен" : "занят")}");
        Console.WriteLine($"  Внутренний тег: {GetInternalTag()}");
        Console.WriteLine($"  Создано: {CreatedAt:dd.MM.yyyy HH:mm}");
    }

    public string StorageKey => $"courier:{Id}";
    public void Save() => Console.WriteLine($"[Courier.Save] Сохранено: {StorageKey}");
    public string GetDescription() => $"{FullName}, рейтинг {Rating:F1}";

    public override string GetShortDescription()
        => $"[Курьер {Id}] {FullName} ({Rating:F1}★)";
}