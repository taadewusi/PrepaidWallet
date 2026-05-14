using PrepaidWallet.DTOs;

namespace PrepaidWallet.Services;

public interface IPrepaidUserService
{
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<UserResponse?> GetUserByIdAsync(int id);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    Task<UserResponse?> UpdateUserAsync(int id, UpdateUserRequest request);
    Task<bool> DeactivateUserAsync(int id);
    Task<bool> ActivateUserAsync(int id);

    Task<BalanceResponse> CreditBalanceAsync(int userId, AdjustBalanceRequest request);
    Task<BalanceResponse> DebitBalanceAsync(int userId, AdjustBalanceRequest request);

    Task<IEnumerable<TransactionResponse>> GetTransactionHistoryAsync(int userId);
    Task<IEnumerable<TransactionResponse>> GetAllTransactionsAsync();
}