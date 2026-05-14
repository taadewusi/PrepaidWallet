using Microsoft.AspNetCore.Mvc;
using PrepaidWallet.DTOs;
using PrepaidWallet.Services;

namespace PrepaidWallet.Controllers;

[ApiController]
[Route("api/users/{userId:int}/balance")]
[Produces("application/json")]
public class BalanceController : ControllerBase
{
    private readonly IPrepaidUserService _service;

    public BalanceController(IPrepaidUserService service)
    {
        _service = service;
    }

    /// <summary>Credit (increase) a user's balance</summary>
    [HttpPost("credit")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Credit(int userId, [FromBody] AdjustBalanceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _service.CreditBalanceAsync(userId, request);
            return Ok(ApiResponse<BalanceResponse>.Ok(result,
                $"Balance credited successfully. New balance: {result.NewBalance:N2}"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<BalanceResponse>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<BalanceResponse>.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<BalanceResponse>.Fail(ex.Message));
        }
    }

    /// <summary>Debit (reduce) a user's balance</summary>
    [HttpPost("debit")]
    [ProducesResponseType(typeof(ApiResponse<BalanceResponse>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Debit(int userId, [FromBody] AdjustBalanceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _service.DebitBalanceAsync(userId, request);
            return Ok(ApiResponse<BalanceResponse>.Ok(result,
                $"Balance debited successfully. New balance: {result.NewBalance:N2}"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<BalanceResponse>.Fail(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<BalanceResponse>.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<BalanceResponse>.Fail(ex.Message));
        }
    }
}
