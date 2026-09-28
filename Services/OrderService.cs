using WtLab2_2oop_Ivanov.Models;

namespace WtLab2_2oop_Ivanov.Services;

public class OrderService
{
    private readonly List<Order> _orders = new();
    private int _nextId = 1;

    public int Count => _orders.Count;
    public IEnumerable<Order> GetAll() => _orders;
    public Order? GetById(int id) => _orders.FirstOrDefault(o => o.Id == id);

    // ПЕРЕГРУЗКА №1
    public Order CreateOrder(string customerName, string address)
        => CreateOrder(customerName, address, new List<Dish>());

    // ПЕРЕГРУЗКА №2
    public Order CreateOrder(string customerName, string address, Dish dish)
        => CreateOrder(customerName, address, new List<Dish> { dish });

    // ПЕРЕГРУЗКА №3
    public Order CreateOrder(string customerName, string address, List<Dish> dishes)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Имя клиента не может быть пустым.", nameof(customerName));
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Адрес не может быть пустым.", nameof(address));
        if (dishes is null)
            throw new ArgumentException("Список блюд не может быть null.", nameof(dishes));

        var order = new Order(_nextId++)
        {
            CustomerName = customerName,
            Address = address,
            Dishes = new List<Dish>(dishes)
        };
        _orders.Add(order);
        return order;
    }
}