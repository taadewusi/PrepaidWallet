namespace PrepaidWallet.Models;

public enum TransactionType
{
    Credit = 1,
    Debit = 2
}

public class BalanceTransaction
{
    public int Id { get; set; }

    public int PrepaidUserId { get; set; }

    public TransactionType TransactionType { get; set; }

    public decimal Amount { get; set; }

    public decimal BalanceBefore { get; set; }

    public decimal BalanceAfter { get; set; }

    public string? Remark { get; set; }

    public string OperatorName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public PrepaidUser? PrepaidUser { get; set; }
}
