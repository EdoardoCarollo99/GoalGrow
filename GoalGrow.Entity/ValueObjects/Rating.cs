using System.ComponentModel.DataAnnotations;

namespace GoalGrow.Entity.ValueObjects
{
    /// <summary>
    /// Value Object per rappresentare rating e recensioni
    /// </summary>
    public record Rating
    {
        [Range(1, 5)]
        public int Value { get; init; }

        [MaxLength(1000)]
        public string Review { get; init; } = string.Empty;

        public DateTime? ReviewDate { get; init; }

        public Rating() { }

        public Rating(int value, string review = "", DateTime? reviewDate = null)
        {
            if (value < 1 || value > 5)
                throw new ArgumentOutOfRangeException(nameof(value), "Rating must be between 1 and 5");

            Value = value;
            Review = review;
            ReviewDate = reviewDate ?? DateTime.UtcNow;
        }

        public static Rating FromValue(int value) => new(value);

        public bool IsPositive => Value >= 4;
        public bool IsNegative => Value <= 2;
        public bool IsNeutral => Value == 3;

        public override string ToString() => $"{Value}/5 stars" + (string.IsNullOrEmpty(Review) ? "" : $": {Review}");
    }
}
