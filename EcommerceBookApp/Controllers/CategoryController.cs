using EcommerceBookApp.DataAccess;
using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBookApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOW;
        public CategoryController(IUnitOfWork unitOW)
        {
            _unitOW = unitOW;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objCatList = _unitOW.Category.GetAll();
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
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CustomError", "The DisplayOrder cannot accept this Name, as they are the same. ");
            }
            if (ModelState.IsValid)
            {
                _unitOW.Category.Add(obj); //creating a method that will be pushed to database
                _unitOW.Save(); // pushing to database by SaveChanges command
                TempData["success"] = "Category has been made successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //var categoryFromDb = _db.Categories.Find(id);
            var categoryFromDb = _unitOW.Category.GetFirstOrDefault(u=>u.Id==id);


            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CustomError", "The DisplayOrder cannot accept this Name, as they are the same. ");
            }
            if (ModelState.IsValid)
            {
                _unitOW.Category.Update(obj); //creating a method that will be pushed to database
                _unitOW.Save(); // pushing to database by SaveChanges command
                TempData["success"] = "Category was updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //var categoryFromDb = _db.Categories.Find(id);
            var categoryFromDb = _unitOW.Category.GetFirstOrDefault(u => u.Id == id);


            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOW.Category.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOW.Category.Remove(obj); //creating a method that will be pushed to database
            _unitOW.Save(); // pushing to database by SaveChanges command
            TempData["success"] = "Category was deleted successfully";
            return RedirectToAction("Index");

            
        }

    }
}
