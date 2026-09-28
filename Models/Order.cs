namespace WtLab2_2oop_Ivanov.Models;

public enum OrderStatus
{
    Created, Cooking, ReadyForDelivery, InDelivery, Delivered, Cancelled
}

public class Order : BaseEntity
{
    public string CustomerName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<Dish> Dishes { get; set; } = new();
    public OrderStatus Status { get; set; } = OrderStatus.Created;
    public int? CourierId { get; set; }

    public decimal TotalPrice => Dishes.Sum(d => d.Price);

    // ★ ОБЯЗАТЕЛЬНЫЙ конструктор — Id через параметр ★
    public Order(int id)
    {
        Id = id;
        SetInternalTag($"ORDER-{Id:D4}");
    }

    public override void PrintInfo()
    {
        Console.WriteLine($"Заказ №{Id}: {CustomerName}");
        Console.WriteLine($"  Адрес: {Address}");
        Console.WriteLine($"  Блюд: {Dishes.Count}");
        Console.WriteLine($"  Сумма: {TotalPrice:C}");
        Console.WriteLine($"  Статус: {Status}");
        Console.WriteLine($"  Курьер: {(CourierId?.ToString() ?? "не назначен")}");
        Console.WriteLine($"  Внутренний тег: {GetInternalTag()}");
    }

    public override string GetShortDescription()
        => $"[Заказ {Id}] {CustomerName} — {TotalPrice:C} ({Status})";
}