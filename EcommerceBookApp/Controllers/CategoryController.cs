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
        //GET
        public IActionResult Create()
        {
            return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CustomError", "The DisplayOrder cannot accept this Name. ")
            }
            if(ModelState.IsValid)
            { 
            _db.Categories.Add(obj); //creating a method that will be pushed to database
            _db.SaveChanges(); // pushing to database by SaveChanges command
            return RedirectToAction("Index");
            }
            return View(obj);
        }

    }
}
