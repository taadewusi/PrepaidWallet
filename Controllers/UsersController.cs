using Microsoft.AspNetCore.Mvc;
using PrepaidWallet.DTOs;
using PrepaidWallet.Services;

namespace PrepaidWallet.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IPrepaidUserService _service;

    public UsersController(IPrepaidUserService service)
    {
        _service = service;
    }

    /// <summary>Get all prepaid users</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserResponse>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _service.GetAllUsersAsync();
        return Ok(ApiResponse<IEnumerable<UserResponse>>.Ok(users));
    }

    /// <summary>Get a single prepaid user by ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _service.GetUserByIdAsync(id);
        if (user is null)
            return NotFound(ApiResponse<UserResponse>.Fail($"User with ID {id} not found."));

        return Ok(ApiResponse<UserResponse>.Ok(user));
    }

    /// <summary>Create a new prepaid user</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _service.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                ApiResponse<UserResponse>.Ok(user, "User created successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<UserResponse>.Fail(ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<UserResponse>.Fail(ex.Message));
        }
    }

    /// <summary>Update a prepaid user's profile (name, phone, email)</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _service.UpdateUserAsync(id, request);
        if (user is null)
            return NotFound(ApiResponse<UserResponse>.Fail($"User with ID {id} not found."));

        return Ok(ApiResponse<UserResponse>.Ok(user, "User updated successfully."));
    }

    /// <summary>Deactivate a prepaid user</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Deactivate(int id)
    {
        var success = await _service.DeactivateUserAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.Fail($"User with ID {id} not found."));

        return NoContent();
    }
}
