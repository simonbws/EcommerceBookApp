using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using EcommerceBookApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Security.Claims;

namespace EcommerceBookAppWeb.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOW; 
    //we are creating products which are going to be displayed on home page
    //unit of work here to get access of all the products
    //we need collection of a products and send them to the view
    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOW)
    {
        _logger = logger;
        _unitOW = unitOW;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> prodList = _unitOW.Product.GetAll(includeProperties: "Category,CoverType"); //we need to retrieve all of the products

        return View(prodList);
    }
    // by adding that only authorized user can access to post action method

    public IActionResult Details(int productId)
    {
        ShopCart ShopCartObj = new()
        {
            Count = 1,
            ProductId = productId, // we added this id because it is in View, and with product below this will be populated
            Product = _unitOW.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,CoverType") //we need to retrieve all of the products
        };
        return View(ShopCartObj);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult Details(ShopCart shopCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        shopCart.AppUserId = claim.Value;// if claim is null that mean that the user is not logged in

        ShopCart cartFromDatabase = _unitOW.ShopCart.GetFirstOrDefault(u => u.AppUserId == claim.Value && u.ProductId == shopCart.ProductId);
           
        if (cartFromDatabase == null)
        {
            _unitOW.ShopCart.Add(shopCart);
        }
        else
        {
            _unitOW.ShopCart.IncrCounter(cartFromDatabase, shopCart.Count);
        }
        

        
        //that we extract user id from claims identity
        //here we can add this to database
        _unitOW.Save();
        return RedirectToAction(nameof(Index));
    }
    

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}