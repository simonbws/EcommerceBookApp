using Microsoft.AspNetCore.Mvc;

namespace EcommerceBookAppWeb.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
