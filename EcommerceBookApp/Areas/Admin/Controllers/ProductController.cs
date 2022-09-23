using EcommerceBookApp.DataAccess;
using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using EcommerceBookApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace EcommerceBookAppWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOW;
    private readonly IWebHostEnvironment _hostEnvironment;


    public ProductController(IUnitOfWork unitOW, IWebHostEnvironment hostEnvironment)
    {
        _unitOW = unitOW;
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
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
            //to sie wywoluje gdy etujemy produkt z produkt listy
            productViewModel.Product = _unitOW.Product.GetFirstOrDefault(u => u.Id == id);
            return View(productViewModel);

            //return View(productViewModel);
            //update product
        }

        //return View(productViewModel);
    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductViewModel obj, IFormFile? file)
    {

        if (ModelState.IsValid)
        {

            string wwwPath = _hostEnvironment.WebRootPath;
            if (file != null) //if file is not null, we will upload the file
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwPath, @"images\productsImg");
                var extension = Path.GetExtension(file.FileName);

                if (obj.Product.ImagePath != null)
                {
                    var oldImagePath = Path.Combine(wwwPath, obj.Product.ImagePath.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath)) //we chech if sth is in this Path if yes, remove it
                    {
                        System.IO.File.Delete(oldImagePath); //old image will be removed
                    }
                }
                //using is to upload file to product folder
                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                obj.Product.ImagePath = @"\images\productsImg\" + fileName + extension; // after the old image was removed, here is updated
            }
            if (obj.Product.Id == 0)
            {
                _unitOW.Product.Add(obj.Product);
            }
            else
            {
                _unitOW.Product.Update(obj.Product);
            }
             //creating a method that will be pushed to database or updated
            _unitOW.Save(); // pushing to database by SaveChanges command
            TempData["success"] = "Product was updated successfully";
            return RedirectToAction("Index");
        }
        return View(obj);
    }

    //GET
    //public IActionResult Delete(int? id)
    ////{
    ////    if (id == null || id == 0)
    ////    {
    ////        return NotFound();
    ////    }

    ////    var CoverTypeFromDb = _unitOW.CoverType.GetFirstOrDefault(u => u.Id == id);


    ////    if (CoverTypeFromDb == null)
    ////    {
    ////        return NotFound();
    ////    }
    ////    return View(CoverTypeFromDb);
    ////}
    ////POST
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public IActionResult DeletePost(int? id)
    //{
    //    var obj = _unitOW.CoverType.GetFirstOrDefault(u => u.Id == id);
    //    if (obj == null)
    //    {
    //        return NotFound();
    //    }

    //    _unitOW.CoverType.Remove(obj); //creating a method that will be pushed to database
    //    _unitOW.Save(); // pushing to database by SaveChanges command
    //    TempData["success"] = "CoverType was deleted successfully";
    //    return RedirectToAction("Index");


    //}

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var productList = _unitOW.Product.GetAll(includeProperties:"Category,CoverType");
        return Json(new { data = productList });
    }
    //POST
    [HttpDelete]
    
    public IActionResult Delete(int? id)
    {
        var obj = _unitOW.Product.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImagePath.TrimStart('\\'));
        if (System.IO.File.Exists(oldImagePath)) //we chech if sth is in this Path if yes, remove it
        {
            System.IO.File.Delete(oldImagePath); //old image will be removed
        }

        _unitOW.Product.Remove(obj); //creating a method that will be pushed to database
        _unitOW.Save(); // pushing to database by SaveChanges command
        return Json(new { success = true, message = "Delete successful" });
        


    }
    #endregion

}
