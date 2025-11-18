namespace GoalGrow.Entity.ValueObjects
{
    /// <summary>
    /// Value Object per rappresentare un periodo di date
    /// </summary>
    public record DateRange
    {
        public DateTime Start { get; init; }
        public DateTime? End { get; init; }

        public DateRange(DateTime start, DateTime? end = null)
        {
            if (end.HasValue && end.Value < start)
                throw new ArgumentException("End date cannot be before start date");

            Start = start;
            End = end;
        }

        public bool IsActive => !End.HasValue || End.Value >= DateTime.UtcNow;

        public bool IsExpired => End.HasValue && End.Value < DateTime.UtcNow;

        public int DaysRemaining => End.HasValue ? (End.Value - DateTime.UtcNow).Days : int.MaxValue;

        public int TotalDays => End.HasValue ? (End.Value - Start).Days : int.MaxValue;

        public bool Contains(DateTime date) => date >= Start && (!End.HasValue || date <= End.Value);

        public override string ToString() => End.HasValue 
            ? $"{Start:yyyy-MM-dd} to {End.Value:yyyy-MM-dd}" 
            : $"From {Start:yyyy-MM-dd}";
    }
}
