using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.ValueObjects
{
    /// <summary>
    /// Value Object per rappresentare denaro con valuta
    /// </summary>
    [ComplexType]
    public record Money
    {
        public decimal Amount { get; init; }
        
        public string Currency { get; init; } = "EUR";

        public Money() { }

        public Money(decimal amount, string currency = "EUR")
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Zero(string currency = "EUR") => new(0, currency);

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot add money with different currencies: {Currency} and {other.Currency}");
            
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot subtract money with different currencies: {Currency} and {other.Currency}");
            
            return new Money(Amount - other.Amount, Currency);
        }

        public Money Multiply(decimal factor) => new(Amount * factor, Currency);

        public decimal Percentage(Money total)
        {
            if (total.Amount == 0) return 0;
            return (Amount / total.Amount) * 100;
        }

        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}
