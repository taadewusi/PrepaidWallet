using Microsoft.AspNetCore.Mvc;
using PrepaidWallet.DTOs;
using PrepaidWallet.Services;

namespace PrepaidWallet.Controllers;

public class UsersViewController : Controller
{
    private readonly IPrepaidUserService _service;

    public UsersViewController(IPrepaidUserService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _service.GetAllUsersAsync();
        return View(users);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        if (!ModelState.IsValid) return View(request);
        try
        {
            await _service.CreateUserAsync(request);
            TempData["Success"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(request);
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _service.GetUserByIdAsync(id);
        if (user is null) return NotFound();

        var transactions = await _service.GetTransactionHistoryAsync(id);
        ViewBag.Transactions = transactions;
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Credit(int id, AdjustBalanceRequest request)
    {
        try
        {
            var result = await _service.CreditBalanceAsync(id, request);
            TempData["Success"] = $"₦{request.Amount:N2} credited. New balance: ₦{result.NewBalance:N2}";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Debit(int id, AdjustBalanceRequest request)
    {
        try
        {
            var result = await _service.DebitBalanceAsync(id, request);
            TempData["Success"] = $"₦{request.Amount:N2} debited. New balance: ₦{result.NewBalance:N2}";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        await _service.DeactivateUserAsync(id);
        TempData["Success"] = "User deactivated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(int id)
    {
        await _service.ActivateUserAsync(id);
        TempData["Success"] = "User activated.";
        return RedirectToAction(nameof(Index));
    }
}