using EcommerceBookApp.Database;
using EcommerceBookApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBookApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;
        public CategoryController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objCatList = _db.Categories;
            return View(objCatList);
        }
    }
}
