﻿using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceBookAppWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOW; // we need to retrieve all the shoping card
        //so thats why we are referencing to IUnitOfWork by dependency injection
        public ShopCartViewModel ShopCartViewModel { get; set; }
        public int OrderTotal { get; set; }
        public CartController(IUnitOfWork unitOW)
        {
            _unitOW = unitOW;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity; // first get the identity
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //we need to populate all shop cart view model
            ShopCartViewModel = new ShopCartViewModel()
            {
                ListCart = _unitOW.ShopCart.GetAll(u => u.AppUserId == claim.Value, includeProperties: "Product") // filter our records
            };
            foreach (var cart in ShopCartViewModel.ListCart)
            {
                cart.Price = PriceByQuantity(cart.Count, cart.Product.Price,
                    cart.Product.Price50, cart.Product.Price100);
                ShopCartViewModel.CartTotal+= (cart.Price * cart.Count);
            }

            return View(ShopCartViewModel);
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOW.ShopCart.GetFirstOrDefault(u => u.Id == cartId); // we need to retrieve from db
            _unitOW.ShopCart.IncrCounter(cart, 1); //increasing by one
            _unitOW.Save(); // only if we invoke save method, it will be shown in database

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cart = _unitOW.ShopCart.GetFirstOrDefault(u => u.Id == cartId); // we need to retrieve from db
            _unitOW.ShopCart.DecrCounter(cart, 1); //increasing by one
            _unitOW.Save(); // only if we invoke save method, it will be shown in database

            return RedirectToAction(nameof(Index));
        }

        private double PriceByQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else
            {
                if (quantity <= 100)
                {
                    return price50;
                }
                return price100;
            }
        }
    }
}
