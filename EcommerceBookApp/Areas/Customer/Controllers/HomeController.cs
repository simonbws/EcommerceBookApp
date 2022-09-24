using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using EcommerceBookApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

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

    public IActionResult Details(int id)
    {
        ShopCart ShopCartObj = new()
        {
            Count = 1,
            Product = _unitOW.Product.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType") //we need to retrieve all of the products
        };
        return View(ShopCartObj);
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