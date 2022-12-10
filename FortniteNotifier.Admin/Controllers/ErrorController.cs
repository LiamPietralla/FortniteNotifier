using Microsoft.AspNetCore.Mvc;

namespace FortniteNotifier.Admin.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
