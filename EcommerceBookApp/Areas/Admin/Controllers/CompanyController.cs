using EcommerceBookApp.DataAccess;
using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using EcommerceBookApp.Models.ViewModels;
using EcommerceBookApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EcommerceBookAppWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOW;
    


    public CompanyController(IUnitOfWork unitOW)
    {
        _unitOW = unitOW;
        
    }

    public IActionResult Index()
    {
        return View();
    }

    //GET
    public IActionResult Upsert(int? id)
    {
        Company company = new();      

        if (id == null || id == 0)
        {
            //if id is null we want to return company
            return View(company);
        }
        else
        {
            //if id is populated we will load our company
            company = _unitOW.Company.GetFirstOrDefault(u => u.Id == id);
            return View(company); // to the view we return back to the company model

            
        }
    
    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Company obj, IFormFile? file)
    {

        if (ModelState.IsValid)
        {
            
            if (obj.Id == 0)
            {
                _unitOW.Company.Add(obj); // if is 0 we will add company
                TempData["success"] = "Company has been created successfully";
            }
            else
            {
                _unitOW.Company.Update(obj); //else we will update
                TempData["success"] = "Company has been updated successfully";

            }
            _unitOW.Save();

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
        var companyList = _unitOW.Company.GetAll();
        return Json(new { data = companyList });
    }
    //POST
    [HttpDelete]
    
    public IActionResult Delete(int? id)
    {
        var obj = _unitOW.Company.GetFirstOrDefault(u => u.Id == id);
        if (obj == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }


        _unitOW.Company.Remove(obj); //creating a method that will be pushed to database
        _unitOW.Save(); // pushing to database by SaveChanges command
        return Json(new { success = true, message = "Delete successful" });
        


    }
    #endregion

}
