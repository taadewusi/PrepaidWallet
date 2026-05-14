using Microsoft.EntityFrameworkCore;
using PrepaidWallet.Data;
using PrepaidWallet.DTOs;
using PrepaidWallet.Models;

namespace PrepaidWallet.Services;

public class PrepaidUserService : IPrepaidUserService
{
    private readonly AppDbContext _db;

    public PrepaidUserService(AppDbContext db)
    {
        _db = db;
    }

    // ── Users ──────────────────────────────────────────────────────────────

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        return await _db.PrepaidUsers
            .AsNoTracking()
            .Select(u => MapToUserResponse(u))
            .ToListAsync();
    }

    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        var user = await _db.PrepaidUsers.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        return user is null ? null : MapToUserResponse(user);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        // Validate unique phone
        bool phoneExists = await _db.PrepaidUsers
            .AnyAsync(u => u.PhoneNumber == request.PhoneNumber);
        if (phoneExists)
            throw new InvalidOperationException($"Phone number '{request.PhoneNumber}' is already registered.");

        // Validate unique email if provided
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            bool emailExists = await _db.PrepaidUsers
                .AnyAsync(u => u.Email == request.Email);
            if (emailExists)
                throw new InvalidOperationException($"Email '{request.Email}' is already registered.");
        }

        if (request.InitialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative.");

        var user = new PrepaidUser
        {
            FullName = request.FullName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = request.Email?.Trim() ?? string.Empty,
            Balance = request.InitialBalance,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.PrepaidUsers.Add(user);
        await _db.SaveChangesAsync();
        return MapToUserResponse(user);
    }

    public async Task<UserResponse?> UpdateUserAsync(int id, UpdateUserRequest request)
    {
        var user = await _db.PrepaidUsers.FindAsync(id);
        if (user is null) return null;

        user.FullName = request.FullName.Trim();
        user.PhoneNumber = request.PhoneNumber.Trim();
        user.Email = request.Email?.Trim() ?? string.Empty;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapToUserResponse(user);
    }

    public async Task<bool> DeactivateUserAsync(int id)
    {
        var user = await _db.PrepaidUsers.FindAsync(id);
        if (user is null) return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivateUserAsync(int id)
    {
        var user = await _db.PrepaidUsers.FindAsync(id);
        if (user is null) return false;

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Balance ────────────────────────────────────────────────────────────

    public async Task<BalanceResponse> CreditBalanceAsync(int userId, AdjustBalanceRequest request)
    {
        ValidateAdjustmentRequest(request);

        var user = await _db.PrepaidUsers.FindAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

        if (!user.IsActive)
            throw new InvalidOperationException("Cannot credit an inactive user account.");

        decimal balanceBefore = user.Balance;
        user.Balance += request.Amount;
        user.UpdatedAt = DateTime.UtcNow;

        var txn = BuildTransaction(user, TransactionType.Credit, request, balanceBefore);
        _db.BalanceTransactions.Add(txn);
        await _db.SaveChangesAsync();

        return BuildBalanceResponse(user, txn, balanceBefore, "Credit");
    }

    public async Task<BalanceResponse> DebitBalanceAsync(int userId, AdjustBalanceRequest request)
    {
        ValidateAdjustmentRequest(request);

        var user = await _db.PrepaidUsers.FindAsync(userId)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

        if (!user.IsActive)
            throw new InvalidOperationException("Cannot debit an inactive user account.");

        if (user.Balance < request.Amount)
            throw new InvalidOperationException(
                $"Insufficient balance. Current balance: {user.Balance:N2}, Requested debit: {request.Amount:N2}.");

        decimal balanceBefore = user.Balance;
        user.Balance -= request.Amount;
        user.UpdatedAt = DateTime.UtcNow;

        var txn = BuildTransaction(user, TransactionType.Debit, request, balanceBefore);
        _db.BalanceTransactions.Add(txn);
        await _db.SaveChangesAsync();

        return BuildBalanceResponse(user, txn, balanceBefore, "Debit");
    }

    // ── Transactions ───────────────────────────────────────────────────────

    public async Task<IEnumerable<TransactionResponse>> GetTransactionHistoryAsync(int userId)
    {
        bool userExists = await _db.PrepaidUsers.AnyAsync(u => u.Id == userId);
        if (!userExists)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        return await _db.BalanceTransactions
            .AsNoTracking()
            .Include(t => t.PrepaidUser)
            .Where(t => t.PrepaidUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => MapToTransactionResponse(t))
            .ToListAsync();
    }

    public async Task<IEnumerable<TransactionResponse>> GetAllTransactionsAsync()
    {
        return await _db.BalanceTransactions
            .AsNoTracking()
            .Include(t => t.PrepaidUser)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => MapToTransactionResponse(t))
            .ToListAsync();
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static void ValidateAdjustmentRequest(AdjustBalanceRequest request)
    {
        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.OperatorName))
            throw new ArgumentException("Operator name is required.");
    }

    private static BalanceTransaction BuildTransaction(
        PrepaidUser user,
        TransactionType type,
        AdjustBalanceRequest request,
        decimal balanceBefore) =>
        new()
        {
            PrepaidUserId = user.Id,
            TransactionType = type,
            Amount = request.Amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = user.Balance,
            Remark = request.Remark,
            OperatorName = request.OperatorName.Trim(),
            CreatedAt = DateTime.UtcNow
        };

    private static BalanceResponse BuildBalanceResponse(
        PrepaidUser user,
        BalanceTransaction txn,
        decimal balanceBefore,
        string type) =>
        new()
        {
            UserId = user.Id,
            FullName = user.FullName,
            PreviousBalance = balanceBefore,
            NewBalance = user.Balance,
            TransactionType = type,
            Amount = txn.Amount,
            Remark = txn.Remark,
            OperatorName = txn.OperatorName,
            Timestamp = txn.CreatedAt
        };

    private static UserResponse MapToUserResponse(PrepaidUser u) =>
        new()
        {
            Id = u.Id,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            Email = u.Email,
            Balance = u.Balance,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        };

    private static TransactionResponse MapToTransactionResponse(BalanceTransaction t) =>
        new()
        {
            Id = t.Id,
            PrepaidUserId = t.PrepaidUserId,
            UserFullName = t.PrepaidUser?.FullName ?? string.Empty,
            TransactionType = t.TransactionType.ToString(),
            Amount = t.Amount,
            BalanceBefore = t.BalanceBefore,
            BalanceAfter = t.BalanceAfter,
            Remark = t.Remark,
            OperatorName = t.OperatorName,
            CreatedAt = t.CreatedAt
        };
}