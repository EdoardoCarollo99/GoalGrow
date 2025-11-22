namespace GoalGrow.API.DTOs.Responses
{
    /// <summary>
    /// Wallet information for investor users
    /// </summary>
    public class WalletResponse
    {
        public Guid UserId { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal TotalDeposited { get; set; }
        public decimal TotalWithdrawn { get; set; }
        public decimal TotalInvested { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal AvailableForInvestment { get; set; }
        public DateTime LastTransactionDate { get; set; }
        public int TransactionCount { get; set; }
    }

    /// <summary>
    /// Summary of user's accounts (bank accounts, payment methods)
    /// </summary>
    public class AccountSummaryResponse
    {
        public Guid AccountId { get; set; }
        public string AccountType { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty; // Masked
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "EUR";
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
    }

    /// <summary>
    /// User statistics for admin dashboard
    /// </summary>
    public class UserStatsResponse
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int InvestorCount { get; set; }
        public int ConsultantCount { get; set; }
        public int AdminCount { get; set; }
        public int KycVerifiedUsers { get; set; }
        public int KycPendingUsers { get; set; }
        public decimal TotalPlatformBalance { get; set; }
        public decimal TotalInvestments { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Paginated list of users for admin panel
    /// </summary>
    public class UserListResponse
    {
        public List<UserResponse> Users { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
