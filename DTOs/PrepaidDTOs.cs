namespace PrepaidWallet.DTOs;

// ── User DTOs ──────────────────────────────────────────────────────────────

public class CreateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; } = 0;
}

public class UpdateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UserResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// ── Balance DTOs ───────────────────────────────────────────────────────────

public class AdjustBalanceRequest
{
    /// <summary>Amount to credit or debit (must be greater than 0)</summary>
    public decimal Amount { get; set; }

    /// <summary>Name of the operator performing the action</summary>
    public string OperatorName { get; set; } = string.Empty;

    /// <summary>Optional note / reason for the adjustment</summary>
    public string? Remark { get; set; }
}

public class BalanceResponse
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public decimal PreviousBalance { get; set; }
    public decimal NewBalance { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Remark { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class TransactionResponse
{
    public int Id { get; set; }
    public int PrepaidUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Remark { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// ── Generic wrapper ────────────────────────────────────────────────────────

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
