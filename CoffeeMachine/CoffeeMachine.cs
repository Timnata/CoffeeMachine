using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeMachine.Models;
using CoffeeMachine.Services;

namespace CoffeeMachine
{
    public class CoffeeMachine
    {
        private readonly List<Drink> _drinks;
        private readonly Dictionary<int, int> _coins;
        private readonly ChangeCalculator _changeCalculator;

        public CoffeeMachine()
        {
            _drinks = new List<Drink>
        {
            new Drink("Эспрессо", 100),
            new Drink("Капучино", 150),
            new Drink("Латте", 200)
        };

            _coins = new Dictionary<int, int>
        {
            { 10, 20 },
            { 50, 15 },
            { 100, 10 },
            { 200, 8 },
            { 500, 5 }
        };

            _changeCalculator = new ChangeCalculator(_coins);
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    ShowMenu();
                    var drink = ChooseDrink();
                    if (drink == null) break;

                    int payment = GetPayment(drink.Price);
                    int changeAmount = payment - drink.Price;

                    if (changeAmount > 0)
                    {
                        var (change, success, message) = _changeCalculator.CalculateChange(changeAmount);
                        if (success)
                        {
                            Console.WriteLine($"\nГотовим {drink.Name}...");
                            Console.WriteLine("Ваш напиток готов! Заберите его.");
                            Console.WriteLine("\nСдача:");
                            foreach (var coin in change.OrderByDescending(kv => kv.Key))
                            {
                                Console.WriteLine($"  {coin.Key} руб. x {coin.Value}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"\nОшибка: {message}");
                            Console.WriteLine("Деньги возвращены.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\nГотовим {drink.Name}...");
                        Console.WriteLine("Ваш напиток готов! Приятного кофейного перерыва!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void ShowMenu()
        {
            Console.WriteLine(" КОФЕМАШИНА D");
            Console.WriteLine("Выберите напиток:");
            for (int i = 0; i < _drinks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_drinks[i]}");
            }
            Console.WriteLine("0. Выход");
            Console.WriteLine(" ");
        }

        private Drink? ChooseDrink()
        {
            while (true)
            {
                Console.Write("Ваш выбор: ");
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Пожалуйста, введите число.");
                    continue;
                }

                if (choice == 0) return null;

                if (choice < 1 || choice > _drinks.Count)
                {
                    Console.WriteLine("Неверный выбор напитка.");
                    continue;
                }

                return _drinks[choice - 1];
            }
        }

        private int GetPayment(int price)
        {
            while (true)
            {
                Console.Write($"\nЦена: {price} руб. Внесите сумму (кратно 10): ");
                if (!int.TryParse(Console.ReadLine(), out int payment) || payment < price || payment % 10 != 0)
                {
                    Console.WriteLine("Недостаточно средств или неверная сумма. Попробуйте снова.");
                    continue;
                }

                return payment;
            }
        }

        // Дополнительно: симуляция обслуживания
        public void RestockCoins(int denomination, int count)
        {
            _changeCalculator.Restock(denomination, count);
            Console.WriteLine($"Пополнено: {count} монет/купюр по {denomination} руб.");
        }
    }
}
