using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Models;
using PersonalFinanceManager.Data;
using System.Security.Claims;

namespace PersonalFinanceManager.Controllers
{
    public class WalletController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WalletController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var wallets = await _context.Wallets
                .Where(w => w.UserId == userId)
                .ToListAsync();

            return View(wallets);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Wallet wallet)
        {
            if (ModelState.IsValid)
            {
                wallet.UserId = GetCurrentUserId();
                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(wallet);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null || wallet.UserId != GetCurrentUserId())
            {
                return NotFound();
            }

            return View(wallet);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Wallet wallet)
        {
            if (id != wallet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    wallet.UserId = GetCurrentUserId();
                    _context.Update(wallet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WalletExists(wallet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(wallet);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null || wallet.UserId != GetCurrentUserId())
            {
                return NotFound();
            }

            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool WalletExists(int id)
        {
            return _context.Wallets.Any(e => e.Id == id);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim);
        }
    }
} 