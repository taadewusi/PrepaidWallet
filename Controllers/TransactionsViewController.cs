using Microsoft.AspNetCore.Mvc;
using PrepaidWallet.Services;

namespace PrepaidWallet.Controllers;

public class TransactionsViewController : Controller
{
    private readonly IPrepaidUserService _service;
    public TransactionsViewController(IPrepaidUserService service) => _service = service;

    public async Task<IActionResult> Index()
    {
        var txns = await _service.GetAllTransactionsAsync();
        return View(txns);
    }
}
