using EcommerceBookApp.DataAccess;
using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using EcommerceBookApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBookAppWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]

public class CoverTypeController : Controller
{
    private readonly IUnitOfWork _unitOW;
    public CoverTypeController(IUnitOfWork unitOW)
    {
        _unitOW = unitOW;
    }

    public IActionResult Index()
    {
        IEnumerable<CoverType> objCoverTypeList = _unitOW.CoverType.GetAll();
        return View(objCoverTypeList);
    }
    //GET
    public IActionResult Create()
    {
        return View();
    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CoverType obj)
    {          
        if (ModelState.IsValid)
        {
            _unitOW.CoverType.Add(obj); //creating a method that will be pushed to database
            _unitOW.Save(); // pushing to database by SaveChanges command
            TempData["success"] = "CoverType has been made successfully";
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
        
        var CoverTypeFromDb = _unitOW.CoverType.GetFirstOrDefault(u => u.Id == id);


        if (CoverTypeFromDb == null)
        {
            return NotFound();
        }
        return View(CoverTypeFromDb);
    }
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(CoverType obj)
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
