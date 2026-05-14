using Microsoft.AspNetCore.Mvc;
using PrepaidWallet.DTOs;
using PrepaidWallet.Services;

namespace PrepaidWallet.Controllers;

[ApiController]
[Produces("application/json")]
public class TransactionsController : ControllerBase
{
    private readonly IPrepaidUserService _service;

    public TransactionsController(IPrepaidUserService service)
    {
        _service = service;
    }

    /// <summary>Get all transactions across all users</summary>
    [HttpGet("api/transactions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TransactionResponse>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await _service.GetAllTransactionsAsync();
        return Ok(ApiResponse<IEnumerable<TransactionResponse>>.Ok(transactions));
    }

    /// <summary>Get transaction history for a specific user</summary>
    [HttpGet("api/users/{userId:int}/transactions")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<TransactionResponse>>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByUser(int userId)
    {
        try
        {
            var transactions = await _service.GetTransactionHistoryAsync(userId);
            return Ok(ApiResponse<IEnumerable<TransactionResponse>>.Ok(transactions));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<IEnumerable<TransactionResponse>>.Fail(ex.Message));
        }
    }
}
