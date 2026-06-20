using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
