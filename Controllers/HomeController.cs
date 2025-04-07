using Microsoft.AspNetCore.Mvc;

namespace PersonalFinanceManager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
} 