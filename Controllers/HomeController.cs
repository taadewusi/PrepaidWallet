using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrepaidWallet.Data;
using PrepaidWallet.Models;

namespace PrepaidWallet.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _db.PrepaidUsers.AsNoTracking().ToListAsync();
        var transactions = await _db.BalanceTransactions
            .AsNoTracking()
            .Include(t => t.PrepaidUser)
            .OrderByDescending(t => t.CreatedAt)
            .Take(10)
            .ToListAsync();

        var vm = new DashboardViewModel
        {
            TotalUsers       = users.Count,
            ActiveUsers      = users.Count(u => u.IsActive),
            TotalBalance     = users.Sum(u => u.Balance),
            TotalCredits     = await _db.BalanceTransactions
                                   .Where(t => t.TransactionType == TransactionType.Credit)
                                   .SumAsync(t => t.Amount),
            TotalDebits      = await _db.BalanceTransactions
                                   .Where(t => t.TransactionType == TransactionType.Debit)
                                   .SumAsync(t => t.Amount),
            RecentTransactions = transactions
        };

        return View(vm);
    }
}

public class DashboardViewModel
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal TotalDebits { get; set; }
    public List<BalanceTransaction> RecentTransactions { get; set; } = new();
}
