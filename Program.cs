using System.Diagnostics;
using System.Globalization;
using WtLab2_2oop_Ivanov.Interfaces;
using WtLab2_2oop_Ivanov.Models;
using WtLab2_2oop_Ivanov.Services;

namespace WtLab2_2oop_Ivanov;

internal static class Program
{
    private static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var beBy = new CultureInfo("be-BY");
        CultureInfo.DefaultThreadCurrentCulture = beBy;
        CultureInfo.DefaultThreadCurrentUICulture = beBy;
        Thread.CurrentThread.CurrentCulture = beBy;

        PrintBanner();

        // ═════ БЛОК 1 ═════
        DemonstrateDishLevel1();
        DemonstrateEncapsulation();
        DemonstratePolymorphism();
        DemonstrateServicesLevel3();

        // ═════ БЛОК 2 ═════
        DemonstrateListsLevel1();
        DemonstrateDictionaryLevel2();
        DemonstrateLinqLevel3();

        // ═════ БЛОК 3 ═════
        await DemonstrateAsyncLevel1();
        await DemonstrateAsyncLevel2();
        await DemonstrateAsyncLevel3();

        // ─── Меню ───
        bool running = true;
        while (running)
        {
            PrintMenu();
            Console.Write("Выбор: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1": Console.WriteLine("[Заглушка] Список ресторанов"); break;
                case "2": Console.WriteLine("[Заглушка] Меню ресторана"); break;
                case "3": Console.WriteLine("[Заглушка] Создать заказ"); break;
                case "4": Console.WriteLine("[Заглушка] Список заказов"); break;
                case "5": Console.WriteLine("[Заглушка] Назначить курьера"); break;
                case "6": Console.WriteLine("[Заглушка] Завершить доставку"); break;
                case "7": Console.WriteLine("[Заглушка] Список курьеров"); break;
                case "0": running = false; break;
                default: Console.WriteLine("Неизвестная команда."); break;
            }
        }

        Console.WriteLine("До свидания!");
    }

    // ═══════════════════════════════════════════════════════
    // БЛОК 3, УРОВЕНЬ 1: async / await
    // ═══════════════════════════════════════════════════════
    private static async Task DemonstrateAsyncLevel1()
    {
        Console.WriteLine();
        Console.WriteLine("═══ БЛОК 3 / УРОВЕНЬ 1: async / await ═══");
        Console.WriteLine();

        var sw = Stopwatch.StartNew();

        Console.WriteLine("→ Запуск асинхронной операции...");
        Console.WriteLine();

        Dish dish = await LoadDishAsync(1, "Маргарита", 12.50m, "Пицца", 20);
        Console.WriteLine($"  ✔ Блюдо загружено: {dish.Name} | {dish.Price:C}");
        Console.WriteLine();

        Console.WriteLine("→ Параллельная загрузка 3 блюд (Task.WhenAll):");
        Console.WriteLine();

        var swParallel = Stopwatch.StartNew();

        Task<Dish> t1 = LoadDishAsync(2, "Пепперони", 14.80m, "Пицца", 22);
        Task<Dish> t2 = LoadDishAsync(3, "Кола", 3.50m, "Напиток", 1);
        Task<Dish> t3 = LoadDishAsync(4, "Мисо-суп", 7.50m, "Суп", 7);

        Dish[] loaded = await Task.WhenAll(t1, t2, t3);

        foreach (var d in loaded)
            Console.WriteLine($"  ✔ {d.Name,-15} {d.Price,8:C}");

        swParallel.Stop();
        Console.WriteLine();
        Console.WriteLine($"  ⏱ Параллельно загружено за {swParallel.ElapsedMilliseconds} мс");
        Console.WriteLine($"     (последовательно было бы ~3000 мс — по 1000 мс на каждое)");
        Console.WriteLine();

        sw.Stop();
        Console.WriteLine($"→ Общее время всей операции: {sw.ElapsedMilliseconds} мс");
        Console.WriteLine();
        Console.WriteLine("→ Проверка: Main НЕ использовал .Result / .Wait() — поток не блокировался.");
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════");
    }

    private static async Task<Dish> LoadDishAsync(
        int id, string name, decimal price, string category, int cookingTime)
    {
        Console.WriteLine($"    [async] Загрузка «{name}» началась...");
        await Task.Delay(1000);
        Console.WriteLine($"    [async] Загрузка «{name}» завершена.");
        return new Dish(id, name, price, category, cookingTime);
    }

    // ═══════════════════════════════════════════════════════
    // БЛОК 3, УРОВЕНЬ 2: async Task<List<T>> + try/catch/finally
    // ═══════════════════════════════════════════════════════
    private static async Task DemonstrateAsyncLevel2()
    {
        Console.WriteLine();
        Console.WriteLine("═══ БЛОК 3 / УРОВЕНЬ 2: async Task<List<T>> + try/catch/finally ═══");
        Console.WriteLine();

        Console.WriteLine("→ Сценарий 1: успешная загрузка коллекции");
        Console.WriteLine();

        List<Dish>? dishes = await LoadDishesAsync(source: "database", simulateError: false);

        if (dishes is not null)
        {
            Console.WriteLine($"  ✔ Получено блюд: {dishes.Count}");
            foreach (var d in dishes)
                Console.WriteLine($"    {d.Id}. {d.Name,-15} {d.Price,8:C} [{d.Category}]");
        }
        Console.WriteLine();

        Console.WriteLine("→ Сценарий 2: имитация ошибки при загрузке (simulateError=true)");
        Console.WriteLine();

        List<Dish>? failed = await LoadDishesAsync(source: "database", simulateError: true);

        if (failed is null)
            Console.WriteLine("  ✔ Метод вернул null (ошибка обработана внутри, программа не упала)");
        Console.WriteLine();

        Console.WriteLine("→ Сценарий 3: параллельная загрузка из двух источников");
        Console.WriteLine();

        var sw = Stopwatch.StartNew();

        Task<List<Dish>?> taskDb = LoadDishesAsync("database", simulateError: false);
        Task<List<Dish>?> taskApi = LoadDishesAsync("external API", simulateError: false);

        List<Dish>?[] results = await Task.WhenAll(taskDb, taskApi);

        sw.Stop();

        int total = results.Where(r => r is not null).Sum(r => r!.Count);
        Console.WriteLine($"  ✔ Обе загрузки завершены за {sw.ElapsedMilliseconds} мс");
        Console.WriteLine($"    Всего получено блюд: {total}");
        Console.WriteLine();

        Console.WriteLine("→ Сценарий 4: проверка, что finally выполняется даже при ошибке");
        Console.WriteLine();

        await LoadDishesAsync("database", simulateError: true);

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════");
    }

    private static async Task<List<Dish>?> LoadDishesAsync(string source, bool simulateError)
    {
        var sw = Stopwatch.StartNew();
        Console.WriteLine($"    [async] Загрузка из «{source}» началась...");

        try
        {
            await Task.Delay(1500);

            if (simulateError)
                throw new InvalidOperationException(
                    $"Не удалось подключиться к источнику «{source}»: таймаут соединения.");

            var dishes = new List<Dish>
            {
                new Dish(101, "Маргарита", 12.50m, "Пицца", 20),
                new Dish(102, "Пепперони", 14.80m, "Пицца", 22),
                new Dish(103, "Кола", 3.50m, "Напиток", 1),
                new Dish(104, "Мисо-суп", 7.50m, "Суп", 7),
                new Dish(105, "Филадельфия", 18.90m, "Суши", 15)
            };

            Console.WriteLine($"    [async] Загрузка из «{source}» успешна ({dishes.Count} блюд, {sw.ElapsedMilliseconds} мс).");
            return dishes;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"    [catch] Ошибка: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    [catch] Непредвиденная ошибка: {ex.Message}");
            return null;
        }
        finally
        {
            sw.Stop();
            Console.WriteLine($"    [finally] Загрузка из «{source}» завершена за {sw.ElapsedMilliseconds} мс.");
        }
    }

    // ═══════════════════════════════════════════════════════
    // БЛОК 3, УРОВЕНЬ 3: Task.WhenAll + CancellationToken
    // ═══════════════════════════════════════════════════════
    private static async Task DemonstrateAsyncLevel3()
    {
        Console.WriteLine();
        Console.WriteLine("═══ БЛОК 3 / УРОВЕНЬ 3: Task.WhenAll + CancellationToken ═══");
        Console.WriteLine();

        // ═══════════════════════════════════════════════════
        // Часть 1. Task.WhenAll — параллельный запуск 5 задач
        // ═══════════════════════════════════════════════════
        Console.WriteLine("─── Часть 1. Task.WhenAll: параллельная загрузка 5 источников ───");
        Console.WriteLine();

        var sw = Stopwatch.StartNew();

        Task<Dish> t1 = LoadDishFromSourceAsync("Ресторан A", "Маргарита", 12.50m, "Пицца", 20, delayMs: 800);
        Task<Dish> t2 = LoadDishFromSourceAsync("Ресторан B", "Пепперони", 14.80m, "Пицца", 22, delayMs: 1000);
        Task<Dish> t3 = LoadDishFromSourceAsync("Ресторан C", "Кола", 3.50m, "Напиток", 1, delayMs: 400);
        Task<Dish> t4 = LoadDishFromSourceAsync("Ресторан D", "Мисо-суп", 7.50m, "Суп", 7, delayMs: 1200);
        Task<Dish> t5 = LoadDishFromSourceAsync("Ресторан E", "Филадельфия", 18.90m, "Суши", 15, delayMs: 600);

        Dish[] allDishes = await Task.WhenAll(t1, t2, t3, t4, t5);

        sw.Stop();

        Console.WriteLine();
        Console.WriteLine($"  ✔ Все 5 задач завершены за {sw.ElapsedMilliseconds} мс");
        Console.WriteLine($"    (последовательно было бы ~4000 мс)");
        Console.WriteLine();

        Console.WriteLine("  Результаты:");
        foreach (var d in allDishes.OrderBy(x => x.Name))
            Console.WriteLine($"    • {d.Name,-15} {d.Price,8:C}");
        Console.WriteLine();

        // ═══════════════════════════════════════════════════
        // Часть 2. CancellationToken — отмена операций
        // ═══════════════════════════════════════════════════
        Console.WriteLine("─── Часть 2. CancellationToken: отмена длительной операции ───");
        Console.WriteLine();

        // Сценарий 2.1 — успех до таймаута
        Console.WriteLine("→ Сценарий 2.1: операция 500 мс, таймаут 2000 мс — успех");
        Console.WriteLine();

        using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(2000)))
        {
            await RunLongOperationAsync("Быстрая операция", delayMs: 500, cts.Token);
        }

        Console.WriteLine();

        // Сценарий 2.2 — отмена по таймауту
        Console.WriteLine("→ Сценарий 2.2: операция 3000 мс, таймаут 1000 мс — отмена");
        Console.WriteLine();

        using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1000)))
        {
            try
            {
                await RunLongOperationAsync("Долгая операция", delayMs: 3000, cts.Token);
            }
            catch (OperationCanceledException)
            {
                // уже напечатано внутри RunLongOperationAsync
            }
        }

        Console.WriteLine();

        // Сценарий 2.3 — ручная отмена
        Console.WriteLine("→ Сценарий 2.3: ручная отмена через cts.Cancel() через 500 мс");
        Console.WriteLine();

        using (var cts = new CancellationTokenSource())
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(500);
                cts.Cancel();
            });

            try
            {
                await RunLongOperationAsync("Ручная отмена", delayMs: 5000, cts.Token);
            }
            catch (OperationCanceledException)
            {
                // уже напечатано
            }
        }

        Console.WriteLine();

        // Сценарий 2.4 — параллельные задачи с общим токеном
        Console.WriteLine("→ Сценарий 2.4: 4 параллельные задачи, отмена через 800 мс");
        Console.WriteLine();

        using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(800)))
        {
            var tasks = new[]
            {
                RunLongOperationAsync("Задача 1", delayMs: 2000, cts.Token),
                RunLongOperationAsync("Задача 2", delayMs: 1500, cts.Token),
                RunLongOperationAsync("Задача 3", delayMs: 3000, cts.Token),
                RunLongOperationAsync("Задача 4", delayMs: 1000, cts.Token)
            };

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("  [catch] Все задачи отменены — OperationCanceledException перехвачено");
            }
        }

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════");
    }

    private static async Task<Dish> LoadDishFromSourceAsync(
        string source, string name, decimal price, string category, int cookingTime, int delayMs)
    {
        Console.WriteLine($"    [async] «{source}»: загрузка «{name}» ({delayMs} мс)...");
        await Task.Delay(delayMs);
        Console.WriteLine($"    [async] «{source}»: «{name}» загружено.");
        return new Dish(0, name, price, category, cookingTime);
    }

    private static async Task RunLongOperationAsync(string name, int delayMs, CancellationToken token)
    {
        var sw = Stopwatch.StartNew();
        Console.WriteLine($"    [op] «{name}»: старт (нужно {delayMs} мс)");

        try
        {
            await Task.Delay(delayMs, token);
            sw.Stop();
            Console.WriteLine($"    [op] «{name}»: УСПЕШНО завершено за {sw.ElapsedMilliseconds} мс");
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            Console.WriteLine($"    [op] «{name}»: ОТМЕНЕНО через {sw.ElapsedMilliseconds} мс");
            throw;
        }
    }

    // ═══════════════════════════════════════════════════════
    // БЛОК 1, УРОВЕНЬ 1
    // ═══════════════════════════════════════════════════════
    private static void DemonstrateDishLevel1()
    {
        Console.WriteLine();
        Console.WriteLine("═══ УРОВЕНЬ 1: базовый класс Dish ═══");
        Console.WriteLine();

        Console.WriteLine("→ Способ 1: new + конструктор");
        Dish margherita = new Dish(1, "Маргарита", 12.50m, "Основное", 20);
        margherita.PrintInfo();
        Console.WriteLine();

        Console.WriteLine("→ Способ 2: new + инициализатор объекта");
        Dish pepperoni = new Dish(2, "Пепперони", 1m, "Основное", 22)
        {
            Name = "Пепперони",
            Category = "Основное",
            CookingTimeMinutes = 22
        };
        pepperoni.UpdatePrice(14.80m);
        pepperoni.PrintInfo();
        Console.WriteLine();
    }

    // ═══════════════════════════════════════════════════════
    // БЛОК 1, УРОВЕНЬ 2, ч.1: инкапсуляция
    // ═══════════════════════════════════════════════════════
    private static void DemonstrateEncapsulation()
    {
        Console.WriteLine("═══ УРОВЕНЬ 2: Инкапсуляция ═══");
        Console.WriteLine();

        var dish = new Dish(10, "Суши-сет", 24.90m, "Основное", 25);
        Console.WriteLine($"Цена до: {dish.Price:C}");
        dish.UpdatePrice(27.50m);
        Console.WriteLine($"Цена после UpdatePrice: {dish.Price:C}");

        try { dish.UpdatePrice(-5m); }
        catch (ArgumentException ex) { Console.WriteLine($"✔ Валидация: {ex.Message}"); }
        Console.WriteLine();

        var courier = new Courier(1, "Иванов И.И.", "+375-29-111-22-33");
        try { courier.Rate(7.0); }
        catch (ArgumentOutOfRangeException ex) { Console.WriteLine($"✔ Валидация рейтинга: {ex.Message}"); }
        Console.WriteLine();

        var rest = new Restaurant(1, "Пиццерия «Верона»", "ул. Ленина, 10");
        rest.AddDish(new Dish(1, "Маргарита", 12.50m, "Основное", 20));
        rest.AddDish(new Dish(2, "Пепперони", 14.80m, "Основное", 22));
        try { rest.AddDish(new Dish(3, "Маргарита", 13.20m, "Основное", 21)); }
        catch (InvalidOperationException ex) { Console.WriteLine($"✔ Дубликат: {ex.Message}"); }
        Console.WriteLine();
    }

    // ═══════════════════════════════════════════════════════
    // БЛОК 1, УРОВЕНЬ 2, ч.2: полиморфизм
    // ═══════════════════════════════════════════════════════
    private static void DemonstratePolymorphism()
    {
        Console.WriteLine("═══ УРОВЕНЬ 2: Полиморфизм ═══");
        Console.WriteLine();

        var tokyo = new Restaurant(301, "Суши-бар «Токио»", "пр. Мира, 25");
        tokyo.AddDish(new Dish(401, "Филадельфия", 18.90m, "Основное", 15));
        tokyo.AddDish(new Dish(402, "Мисо-суп", 7.50m, "Суп", 7));

        List<BaseEntity> entities = new()
        {
            new Dish(101, "Маргарита", 12.50m, "Основное", 20),
            new Dish(102, "Кола", 3.50m, "Напиток", 1),
            new Courier(201, "Петров П.П.", "+375-29-444-55-66", 4.5),
            tokyo
        };

        Console.WriteLine("→ Полиморфный вызов PrintInfo():");
        foreach (BaseEntity e in entities)
        {
            e.PrintInfo();
            Console.WriteLine();
        }

        Console.WriteLine("→ Короткие описания:");
        foreach (BaseEntity e in entities)
            Console.WriteLine("  " + e.GetShortDescription());
        Console.WriteLine();

        Console.WriteLine("→ Проверка интерфейса IStorable:");
        foreach (BaseEntity e in entities)
        {
            if (e is IStorable storable)
            {
                storable.Save();
                Console.WriteLine($"  StorageKey: {storable.StorageKey}");
                Console.WriteLine($"  Описание:   {storable.GetDescription()}");
                Console.WriteLine();
            }
        }
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine();
    }

    // ═══════════════════════════════════════════════════════
    // БЛОК 1, УРОВЕНЬ 3: дженерик-сервисы
    // ═══════════════════════════════════════════════════════
    private static void DemonstrateServicesLevel3()
    {
        Console.WriteLine("═══ УРОВЕНЬ 3: Дженерик-сервисы ═══");
        Console.WriteLine();

        IItemService<Dish> dishService = new InMemoryService<Dish>();

        dishService.Add(new Dish(1, "Маргарита", 12.50m, "Основное", 20));
        dishService.Add(new Dish(2, "Пепперони", 14.80m, "Основное", 22));
        dishService.Add(new Dish(3, "Кола", 3.50m, "Напиток", 1));
        dishService.Add(new Dish(4, "Мисо-суп", 7.50m, "Суп", 7));

        Console.WriteLine($"Блюд в сервисе: {dishService.Count}");
        Console.WriteLine();

        Console.WriteLine("→ Полиморфный вызов через IItemService<Dish>:");
        foreach (var d in dishService.GetAll())
            Console.WriteLine("  " + d.GetShortDescription());
        Console.WriteLine();

        Console.WriteLine("→ Find(d => d.Price > 10):");
        foreach (var d in dishService.Find(d => d.Price > 10m))
            Console.WriteLine("  " + d.GetShortDescription());
        Console.WriteLine();

        Console.WriteLine("→ GetAllSorted(d => d.Price):");
        foreach (var d in dishService.GetAllSorted(d => d.Price))
            Console.WriteLine($"  {d.Name,-15} {d.Price,8:C}");
        Console.WriteLine();

        bool removed = dishService.Remove(3);
        Console.WriteLine($"→ Remove(3): {(removed ? "удалено" : "не найдено")}. Осталось: {dishService.Count}");
        Console.WriteLine();

        IItemService<Courier> courierService = new InMemoryService<Courier>();
        courierService.Add(new Courier(1, "Иванов И.И.", "+375-29-111-22-33", 4.8));
        courierService.Add(new Courier(2, "Петров П.П.", "+375-29-444-55-66", 4.5));
        courierService.Add(new Courier(3, "Сидорова А.А.", "+375-29-777-88-99", 4.9));

        Console.WriteLine($"→ Курьеров: {courierService.Count}");
        Console.WriteLine("  Свободные:");
        foreach (var c in courierService.Find(c => c.IsAvailable))
            Console.WriteLine($"    {c.FullName} — {c.Rating:F1}★");
        Console.WriteLine();

        Console.WriteLine("→ Protected-теги:");
        Console.WriteLine($"  Dish.Id=1    → {dishService.GetById(1)?.GetInternalTag()}");
        Console.WriteLine($"  Courier.Id=1 → {courierService.GetById(1)?.GetInternalTag()}");
        Console.WriteLine();

        Console.WriteLine("→ Перегрузка CreateOrder():");
        var orderService = new OrderService();

        var o1 = orderService.CreateOrder("Алексей", "ул. Пушкина, 5");
        var o2 = orderService.CreateOrder("Мария", "пр. Мира, 10",
            new Dish(10, "Суши-сет", 24.90m, "Основное", 25));
        var o3 = orderService.CreateOrder("Иван", "ул. Гагарина, 1", new List<Dish>
        {
            new Dish(11, "Пепперони", 14.80m, "Основное", 22),
            new Dish(12, "Кола", 3.50m, "Напиток", 1)
        });

        Console.WriteLine($"  Заказ 1:  {o1.GetShortDescription()}");
        Console.WriteLine($"  Заказ 2:  {o2.GetShortDescription()}");
        Console.WriteLine($"  Заказ 3:  {o3.GetShortDescription()}");
        Console.WriteLine($"  Всего заказов: {orderService.Count}");
        Console.WriteLine();

        try
        {
            dishService.Add(new Dish(1, "Дубль", 5m, "Основное", 5));
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"✔ Валидация дубликата: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine();
    }

    // ═══════════════════════════════════════════════════════
    // БЛОК 2, УРОВЕНЬ 1: List<T> + foreach
    // ═══════════════════════════════════════════════════════
    private static void DemonstrateListsLevel1()
    {
        Console.WriteLine("═══ БЛОК 2 / УРОВЕНЬ 1: Коллекция List<T> ═══");
        Console.WriteLine();

        List<Dish> dishes = new List<Dish>();

        Console.WriteLine($"→ Коллекция создана. Начальное количество: {dishes.Count}");
        Console.WriteLine();

        dishes.Add(new Dish(1, "Маргарита", 12.50m, "Основное", 20));
        dishes.Add(new Dish(2, "Пепперони", 14.80m, "Основное", 22));
        dishes.Add(new Dish(3, "Кола", 3.50m, "Напиток", 1));
        dishes.Add(new Dish(4, "Мисо-суп", 7.50m, "Суп", 7));
        dishes.Add(new Dish(5, "Филадельфия", 18.90m, "Основное", 15));
        dishes.Add(new Dish(6, "Матча", 5.20m, "Напиток", 3));
        dishes.Add(new Dish(7, "Тирамису", 9.40m, "Десерт", 10));

        Console.WriteLine($"→ Добавлено блюд: {dishes.Count}");
        Console.WriteLine();

        Console.WriteLine("→ Перебор коллекции через foreach:");
        Console.WriteLine();

        int index = 1;
        foreach (Dish dish in dishes)
        {
            Console.WriteLine($"  #{index++}. {dish.Name} | {dish.Category} | {dish.Price:C} | {dish.CookingTimeMinutes} мин.");
        }

        Console.WriteLine();
        Console.WriteLine($"→ Итого блюд в коллекции: {dishes.Count}");
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine();
    }

    // ═══════════════════════════════════════════════════════
    // БЛОК 2, УРОВЕНЬ 2: Dictionary<TKey, TValue> + TryGetValue
    // ═══════════════════════════════════════════════════════
    private static void DemonstrateDictionaryLevel2()
    {
        Console.WriteLine("═══ БЛОК 2 / УРОВЕНЬ 2: Dictionary<TKey, TValue> ═══");
        Console.WriteLine();

        Dictionary<int, Dish> dishesById = new Dictionary<int, Dish>();

        dishesById.Add(1, new Dish(1, "Маргарита", 12.50m, "Основное", 20));
        dishesById.Add(2, new Dish(2, "Пепперони", 14.80m, "Основное", 22));
        dishesById.Add(3, new Dish(3, "Кола", 3.50m, "Напиток", 1));
        dishesById.Add(4, new Dish(4, "Мисо-суп", 7.50m, "Суп", 7));
        dishesById.Add(5, new Dish(5, "Филадельфия", 18.90m, "Основное", 15));

        Console.WriteLine($"→ Словарь создан. Записей: {dishesById.Count}");
        Console.WriteLine();

        Console.WriteLine("→ Прямой доступ по ключу (dishesById[2]):");
        Dish dish2 = dishesById[2];
        Console.WriteLine($"  Id=2 → {dish2.Name} | {dish2.Price:C}");
        Console.WriteLine();

        Console.WriteLine("→ Перебор словаря через foreach:");
        foreach (KeyValuePair<int, Dish> pair in dishesById)
        {
            Console.WriteLine($"  Ключ {pair.Key} → {pair.Value.Name} ({pair.Value.Price:C})");
        }
        Console.WriteLine();

        Console.WriteLine("→ Безопасный поиск через TryGetValue:");
        Console.WriteLine();

        TryFindDish(dishesById, 3);
        TryFindDish(dishesById, 99);

        Console.WriteLine();

        Console.WriteLine("→ Альтернатива через ContainsKey:");
        int searchId = 5;
        if (dishesById.ContainsKey(searchId))
            Console.WriteLine($"  Id={searchId} найден: {dishesById[searchId].Name}");
        else
            Console.WriteLine($"  Id={searchId} не найден.");

        int missingId = 42;
        if (dishesById.ContainsKey(missingId))
            Console.WriteLine($"  Id={missingId} найден: {dishesById[missingId].Name}");
        else
            Console.WriteLine($"  Id={missingId} НЕ найден — обработано.");

        Console.WriteLine();

        Console.WriteLine("→ Прямой доступ к отсутствующему ключу dishesById[99]:");
        try
        {
            var bad = dishesById[99];
            Console.WriteLine($"  (не должно выполниться) {bad.Name}");
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"  ✔ Исключение KeyNotFoundException перехвачено: {ex.Message}");
        }

        Console.WriteLine();

        Console.WriteLine("→ Удаление ключа 3:");
        bool removed = dishesById.Remove(3);
        Console.WriteLine($"  Remove(3): {(removed ? "удалено" : "не найдено")}. Осталось: {dishesById.Count}");
        Console.WriteLine();

        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine();
    }

    private static void TryFindDish(Dictionary<int, Dish> dict, int id)
    {
        if (dict.TryGetValue(id, out Dish? found))
            Console.WriteLine($"  ✔ Id={id} найден: {found!.Name} | {found.Price:C}");
        else
            Console.WriteLine($"  ✘ Id={id} НЕ найден — обработано без исключения.");
    }

    // ═══════════════════════════════════════════════════════
    // БЛОК 2, УРОВЕНЬ 3: LINQ Method Syntax
    // ═══════════════════════════════════════════════════════
    private static void DemonstrateLinqLevel3()
    {
        Console.WriteLine("═══ БЛОК 2 / УРОВЕНЬ 3: LINQ Method Syntax ═══");
        Console.WriteLine();

        List<Dish> dishes = new List<Dish>
        {
            new Dish(1, "Маргарита", 12.50m, "Пицца", 20),
            new Dish(2, "Пепперони", 14.80m, "Пицца", 22),
            new Dish(3, "Кола", 3.50m, "Напиток", 1),
            new Dish(4, "Мисо-суп", 7.50m, "Суп", 7),
            new Dish(5, "Филадельфия", 18.90m, "Суши", 15),
            new Dish(6, "Матча", 5.20m, "Напиток", 3),
            new Dish(7, "Тирамису", 9.40m, "Десерт", 10),
            new Dish(8, "Калифорния", 16.70m, "Суши", 12),
            new Dish(9, "Борщ", 8.30m, "Суп", 25)
        };

        Console.WriteLine($"→ Исходная коллекция: {dishes.Count} блюд");
        Console.WriteLine();

        Console.WriteLine("─── 1. Where: блюда дороже 10 Br ───");
        var expensive = dishes.Where(d => d.Price > 10m).ToList();
        foreach (var d in expensive)
            Console.WriteLine($"  {d.Name,-15} {d.Price,8:C}");
        Console.WriteLine($"  Найдено: {expensive.Count}");
        Console.WriteLine();

        Console.WriteLine("─── 1b. Where: Суши ИЛИ Пицца ───");
        var pizzaOrSushi = dishes
            .Where(d => d.Category == "Пицца" || d.Category == "Суши")
            .ToList();
        foreach (var d in pizzaOrSushi)
            Console.WriteLine($"  {d.Name,-15} [{d.Category}] {d.Price:C}");
        Console.WriteLine($"  Найдено: {pizzaOrSushi.Count}");
        Console.WriteLine();

        Console.WriteLine("─── 2. OrderBy: по цене (возрастание) ───");
        var byPriceAsc = dishes.OrderBy(d => d.Price).ToList();
        foreach (var d in byPriceAsc)
            Console.WriteLine($"  {d.Name,-15} {d.Price,8:C}");
        Console.WriteLine();

        Console.WriteLine("─── 2b. OrderByDescending: по цене (убывание) ───");
        var byPriceDesc = dishes.OrderByDescending(d => d.Price).ToList();
        foreach (var d in byPriceDesc)
            Console.WriteLine($"  {d.Name,-15} {d.Price,8:C}");
        Console.WriteLine();

        Console.WriteLine("─── 2c. OrderBy(Category).ThenBy(Price) ───");
        var byCategoryThenPrice = dishes
            .OrderBy(d => d.Category)
            .ThenBy(d => d.Price)
            .ToList();
        foreach (var d in byCategoryThenPrice)
            Console.WriteLine($"  [{d.Category,-8}] {d.Name,-15} {d.Price,8:C}");
        Console.WriteLine();

        Console.WriteLine("─── 3. Select: только названия ───");
        var names = dishes.Select(d => d.Name).ToList();
        Console.WriteLine("  " + string.Join(", ", names));
        Console.WriteLine();

        Console.WriteLine("─── 3b. Select в анонимный тип ───");
        var brief = dishes
            .Select(d => new { d.Name, d.Price, Info = $"{d.Category}, {d.CookingTimeMinutes} мин." })
            .ToList();
        foreach (var b in brief)
            Console.WriteLine($"  {b.Name,-15} | {b.Price,8:C} | {b.Info}");
        Console.WriteLine();

        Console.WriteLine("─── 4. Агрегатные операции ───");
        Console.WriteLine($"  Count:            {dishes.Count}");
        Console.WriteLine($"  Sum(Price):       {dishes.Sum(d => d.Price):C}");
        Console.WriteLine($"  Average(Price):   {dishes.Average(d => d.Price):C}");
        Console.WriteLine($"  Min(Price):       {dishes.Min(d => d.Price):C}");
        Console.WriteLine($"  Max(Price):       {dishes.Max(d => d.Price):C}");
        Console.WriteLine($"  Min(CookingTime): {dishes.Min(d => d.CookingTimeMinutes)} мин.");
        Console.WriteLine($"  Max(CookingTime): {dishes.Max(d => d.CookingTimeMinutes)} мин.");
        Console.WriteLine();

        int drinksCount = dishes.Count(d => d.Category == "Напиток");
        Console.WriteLine($"  Count(напитки):   {drinksCount}");
        Console.WriteLine();

        Console.WriteLine("─── 5. GroupBy: группировка по категориям ───");
        Console.WriteLine();

        var groups = dishes
            .GroupBy(d => d.Category)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var group in groups)
        {
            Console.WriteLine($"  ▶ Категория: {group.Key}");
            Console.WriteLine($"    Блюд в группе: {group.Count()}");
            Console.WriteLine($"    Средняя цена:  {group.Average(d => d.Price):C}");
            Console.WriteLine($"    Сумма:         {group.Sum(d => d.Price):C}");
            Console.WriteLine($"    Мин:           {group.Min(d => d.Price):C}");
            Console.WriteLine($"    Макс:          {group.Max(d => d.Price):C}");
            Console.WriteLine($"    Состав:");
            foreach (var d in group.OrderByDescending(x => x.Price))
                Console.WriteLine($"      • {d.Name,-15} {d.Price,8:C}");
            Console.WriteLine();
        }

        Console.WriteLine("─── 6. Комбо: Where(>8) + OrderBy(Price) + Select(анонимный) ───");
        var combo = dishes
            .Where(d => d.Price > 8m)
            .OrderBy(d => d.Price)
            .Select(d => new { d.Name, d.Category, d.Price })
            .ToList();

        foreach (var item in combo)
            Console.WriteLine($"  {item.Name,-15} [{item.Category,-8}] {item.Price,8:C}");
        Console.WriteLine();

        Console.WriteLine("─── 7. Итоговая статистика ───");
        Console.WriteLine($"  Всего блюд:      {dishes.Count}");
        Console.WriteLine($"  Категорий:       {groups.Count}");
        Console.WriteLine($"  Общая сумма:     {dishes.Sum(d => d.Price):C}");
        Console.WriteLine($"  Средняя цена:    {dishes.Average(d => d.Price):C}");
        Console.WriteLine($"  Самое дорогое:   {dishes.MaxBy(d => d.Price)!.Name} ({dishes.Max(d => d.Price):C})");
        Console.WriteLine($"  Самое дешёвое:   {dishes.MinBy(d => d.Price)!.Name} ({dishes.Min(d => d.Price):C})");
        Console.WriteLine();

        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine();
    }

    private static void PrintBanner()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║   Служба доставки еды «БыстроЕда» — Лабораторная №2  ║");
        Console.WriteLine("║   Вариант 9: Dish, Order, Courier, Restaurant        ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("──────────── МЕНЮ ────────────");
        Console.WriteLine("1. Список ресторанов");
        Console.WriteLine("2. Меню ресторана");
        Console.WriteLine("3. Создать заказ");
        Console.WriteLine("4. Список заказов");
        Console.WriteLine("5. Назначить курьера");
        Console.WriteLine("6. Завершить доставку");
        Console.WriteLine("7. Список курьеров");
        Console.WriteLine("0. Выход");
        Console.WriteLine("──────────────────────────────");
    }
}