using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

            return View(transactions);
        }

        public async Task<IActionResult> Create()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            ViewBag.Wallets = await _context.Wallets
                .Where(w => w.UserId == userId)
                .ToListAsync();

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

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
} 