using Microsoft.AspNetCore.Mvc;

namespace demo_boeing_peoplesoft.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
