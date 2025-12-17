using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CoffeeMachine.Services
{
    public class ChangeCalculator
    {
        private readonly Dictionary<int, int> _availableCoins;

        public ChangeCalculator(Dictionary<int, int> availableCoins)
        {
            _availableCoins = new Dictionary<int, int>(availableCoins);
        }

        /// Выдаёт сдачу максимально равномерно, используя LINQ.
        /// Стремится распределить сдачу по всем доступным номиналам пропорционально.
    
        public (Dictionary<int, int> change, bool success, string message) CalculateChange(int amount)
        {
            if (amount < 0)
                return (new Dictionary<int, int>(), false, "Сумма сдачи не может быть отрицательной.");

            if (amount == 0)
                return (new Dictionary<int, int>(), true, "Сдача не требуется.");

            var denominations = _availableCoins.Keys.OrderByDescending(k => k).ToList();
            var totalAvailable = _availableCoins.Sum(kv => kv.Key * kv.Value);

            if (totalAvailable < amount)
                return (new Dictionary<int, int>(), false, "Недостаточно монет в автомате для выдачи сдачи.");

            var result = new Dictionary<int, int>();

            // Основная идея: распределить сдачу как можно равномернее
            // Используем LINQ для циклического прохода по номиналам
            var cycle = denominations.Cycle(); // кастомное расширение (см. ниже)

            int remaining = amount;
            foreach (var denom in cycle.Take(denominations.Count * 10)) // ограничиваем итерации
            {
                if (remaining <= 0) break;

                int availableCount = _availableCoins[denom];
                if (availableCount > 0 && remaining >= denom)
                {
                    int coinsToUse = 1; // берём по одной монете за раз для равномерности
                    result[denom] = result.GetValueOrDefault(denom) + coinsToUse;
                    _availableCoins[denom]--;
                    remaining -= denom;
                }
            }

            if (remaining > 0)
            {
                // Если не удалось выдать всю сумму — откатываем изменения
                foreach (var kv in result)
                {
                    _availableCoins[kv.Key] += kv.Value;
                }
                return (new Dictionary<int, int>(), false, "Невозможно выдать точную сдачу имеющимися монетами.");
            }

            return (result, true, "Сдача выдана успешно.");
        }

       
        /// Пополнение монет в автомате
        
        public void Restock(int denomination, int count)
        {
            if (_availableCoins.ContainsKey(denomination))
                _availableCoins[denomination] += count;
            else
                _availableCoins[denomination] = count;
        }
    }

    // Расширение для бесконечного цикла по списку
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> source)
        {
            while (true)
            {
                foreach (var item in source)
                    yield return item;
            }
        }
    }
}
