using CoffeeMachine.Services;

namespace CalculateChange
{
    public class UnitTest1
    {
        [Fact]
        public void CalculateChange_ReturnsCorrectSum()
        {
            var coins = new Dictionary<int, int>
            {
                { 10, 10 },
                { 50, 10 },
                { 100, 10 },
                { 200, 10 },
                { 500, 10 }
            };

            var calculator = new ChangeCalculator(coins);

            var (change, success, _) = calculator.CalculateChange(300);

            Assert.True(success);
            Assert.Equal(300, change.Sum(kv => kv.Key * kv.Value));
        }
    }
}