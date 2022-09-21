using EcommerceBookApp.DataAccess;
using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using EcommerceBookApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommerceBookAppWeb.Areas.Admin.Controllers

{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOW;
        public ProductController(IUnitOfWork unitOW)
        {
            _unitOW = unitOW;
        }

        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverTypeList = _unitOW.CoverType.GetAll();
            return View(objCoverTypeList);
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            ProductViewModel productViewModel = new()
            {
                Product = new(),
                CatList = _unitOW.Category.GetAll().Select(i => new SelectListItem
                {

                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOW.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };


            if (id == null || id == 0)
            {
                //ViewBag.CatList = CatList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                //we want to create product
                return View(productViewModel);
            }
            else
            {
                //update product
            }


            return View(productViewModel);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType obj)
        {

            if (ModelState.IsValid)
            {
                _unitOW.CoverType.Update(obj); //creating a method that will be pushed to database
                _unitOW.Save(); // pushing to database by SaveChanges command
                TempData["success"] = "CoverType was updated successfully";
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

            var CoverTypeFromDb = _unitOW.CoverType.GetFirstOrDefault(u => u.Id == id);


            if (CoverTypeFromDb == null)
            {
                return NotFound();
            }
            return View(CoverTypeFromDb);
        }
        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _unitOW.CoverType.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOW.CoverType.Remove(obj); //creating a method that will be pushed to database
            _unitOW.Save(); // pushing to database by SaveChanges command
            TempData["success"] = "CoverType was deleted successfully";
            return RedirectToAction("Index");


        }

    }
}
