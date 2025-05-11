using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Models;
using System.Security.Claims;

namespace PersonalFinanceManager.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();

            return View(budgets);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Budget budget)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                budget.UserId = userId;
                budget.CreatedAt = DateTime.Now;

                _context.Add(budget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(budget);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null)
            {
                return NotFound();
            }

            return View(budget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Budget budget)
        {
            if (id != budget.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    var existingBudget = await _context.Budgets
                        .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

                    if (existingBudget == null)
                    {
                        return NotFound();
                    }

                    existingBudget.Category = budget.Category;
                    existingBudget.Amount = budget.Amount;
                    existingBudget.Period = budget.Period;
                    existingBudget.StartDate = budget.StartDate;
                    existingBudget.EndDate = budget.EndDate;
                    existingBudget.Description = budget.Description;
                    existingBudget.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetExists(budget.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(budget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null)
            {
                return NotFound();
            }

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CheckStatus(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null)
            {
                return NotFound();
            }

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId &&
                           t.Category == budget.Category &&
                           t.TransactionDate >= budget.StartDate &&
                           t.TransactionDate <= budget.EndDate &&
                           t.Type == "Expense")
                .ToListAsync();

            var totalSpent = transactions.Sum(t => t.Amount);
            var remainingBudget = budget.Amount - totalSpent;
            var percentageUsed = (totalSpent / budget.Amount) * 100;

            ViewBag.TotalSpent = totalSpent;
            ViewBag.RemainingBudget = remainingBudget;
            ViewBag.PercentageUsed = percentageUsed;
            ViewBag.Transactions = transactions;

            return View(budget);
        }

        private bool BudgetExists(int id)
        {
            return _context.Budgets.Any(e => e.Id == id);
        }
    }
} 