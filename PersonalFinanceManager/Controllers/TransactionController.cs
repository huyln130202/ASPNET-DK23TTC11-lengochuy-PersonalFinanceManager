using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Models;
using System.Security.Claims;

namespace PersonalFinanceManager.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var transactions = await _context.Transactions
                .Include(t => t.Wallet)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            // Tính toán thống kê theo tháng
            var currentYear = DateTime.Now.Year;
            var monthlyStats = new List<object>();
            
            for (int month = 1; month <= 12; month++)
            {
                var monthTransactions = transactions
                    .Where(t => t.TransactionDate.Year == currentYear && t.TransactionDate.Month == month)
                    .ToList();

                var income = monthTransactions
                    .Where(t => t.Type == "Income")
                    .Sum(t => t.Amount);

                var expense = monthTransactions
                    .Where(t => t.Type == "Expense")
                    .Sum(t => t.Amount);

                monthlyStats.Add(new
                {
                    Month = month,
                    MonthName = new DateTime(currentYear, month, 1).ToString("MMM"),
                    Income = income,
                    Expense = expense
                });
            }

            ViewBag.MonthlyStats = monthlyStats;
            return View(transactions);
        }

        public async Task<IActionResult> Create()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            ViewBag.WalletId = new SelectList(await _context.Wallets.Where(w => w.UserId == userId).ToListAsync(), "Id", "Name");
            ViewBag.Budgets = await _context.Budgets.Where(b => b.UserId == userId).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var wallet = await _context.Wallets.FindAsync(transaction.WalletId);

                if (wallet == null || wallet.UserId != userId)
                {
                    return NotFound();
                }

                transaction.UserId = userId;
                transaction.CreatedAt = DateTime.Now;

                // Update wallet balance
                if (transaction.Type == "Income")
                {
                    wallet.Balance += transaction.Amount;
                }
                else
                {
                    wallet.Balance -= transaction.Amount;
                }

                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var userWallets = await _context.Wallets
                .Where(w => w.UserId == transaction.UserId)
                .ToListAsync();
            ViewBag.Wallets = userWallets;

            return View(transaction);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }

            ViewBag.Wallets = await _context.Wallets
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    var originalTransaction = await _context.Transactions
                        .Include(t => t.Wallet)
                        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

                    if (originalTransaction == null)
                    {
                        return NotFound();
                    }

                    // Revert original wallet balance
                    if (originalTransaction.Type == "Income")
                    {
                        originalTransaction.Wallet.Balance -= originalTransaction.Amount;
                    }
                    else
                    {
                        originalTransaction.Wallet.Balance += originalTransaction.Amount;
                    }

                    // Update with new values
                    var newWallet = await _context.Wallets.FindAsync(transaction.WalletId);
                    if (newWallet == null || newWallet.UserId != userId)
                    {
                        return NotFound();
                    }

                    // Update new wallet balance
                    if (transaction.Type == "Income")
                    {
                        newWallet.Balance += transaction.Amount;
                    }
                    else
                    {
                        newWallet.Balance -= transaction.Amount;
                    }

                    originalTransaction.Amount = transaction.Amount;
                    originalTransaction.Type = transaction.Type;
                    originalTransaction.Category = transaction.Category;
                    originalTransaction.Description = transaction.Description;
                    originalTransaction.TransactionDate = transaction.TransactionDate;
                    originalTransaction.WalletId = transaction.WalletId;
                    originalTransaction.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Wallets = await _context.Wallets
                .Where(w => w.UserId == transaction.UserId)
                .ToListAsync();

            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }

            // Update wallet balance
            if (transaction.Type == "Income")
            {
                transaction.Wallet.Balance -= transaction.Amount;
            }
            else
            {
                transaction.Wallet.Balance += transaction.Amount;
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Statistics(int? month, int? year)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();

            // Lọc theo tháng/năm nếu có
            if (year.HasValue)
                transactions = transactions.Where(t => t.TransactionDate.Year == year.Value).ToList();
            if (month.HasValue)
                transactions = transactions.Where(t => t.TransactionDate.Month == month.Value).ToList();

            // Tổng thu, chi, chênh lệch
            var totalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
            var difference = totalIncome - totalExpense;

            // Thống kê theo danh mục
            var categoryStats = transactions
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .OrderByDescending(g => g.Total)
                .ToList();

            // Thống kê theo loại
            var typeStats = transactions
                .GroupBy(t => t.Type)
                .Select(g => new { Type = g.Key, Total = g.Sum(t => t.Amount) })
                .ToList();

            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.Difference = difference;
            ViewBag.CategoryStats = categoryStats;
            ViewBag.TypeStats = typeStats;
            ViewBag.Month = month;
            ViewBag.Year = year;

            return View();
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
} 