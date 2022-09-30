using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using EcommerceBookApp.Models.ViewModels;
using EcommerceBookApp.Utility;
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
                ListCart = _unitOW.ShopCart.GetAll(u => u.AppUserId == claim.Value, includeProperties: "Product"), // filter our records
                OrderHeader=new()

            };
            foreach (var cart in ShopCartViewModel.ListCart)
            {
                cart.Price = PriceByQuantity(cart.Count, cart.Product.Price,
                    cart.Product.Price50, cart.Product.Price100);
                ShopCartViewModel.OrderHeader.OrderTotal+= (cart.Price * cart.Count);
            }

            return View(ShopCartViewModel);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity; // first get the identity
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //we need to populate all shop cart view model
            ShopCartViewModel = new ShopCartViewModel()
            {
                ListCart = _unitOW.ShopCart.GetAll(u => u.AppUserId == claim.Value, includeProperties: "Product"), // filter our records
                OrderHeader= new()
            };
            ShopCartViewModel.OrderHeader.AppUser = _unitOW.AppUser.GetFirstOrDefault(
                u => u.Id == claim.Value); // that will retrieve all the app user details for our loggin in user
            //here we can go with properties
            ShopCartViewModel.OrderHeader.Name = ShopCartViewModel.OrderHeader.AppUser.Name;
            ShopCartViewModel.OrderHeader.PhoneNumber = ShopCartViewModel.OrderHeader.AppUser.PhoneNumber;
            ShopCartViewModel.OrderHeader.StreetAddress = ShopCartViewModel.OrderHeader.AppUser.StreetAddress;
            ShopCartViewModel.OrderHeader.State = ShopCartViewModel.OrderHeader.AppUser.State;
            ShopCartViewModel.OrderHeader.PostalCode = ShopCartViewModel.OrderHeader.AppUser.PostalCode;
            // here we need to add get total
            foreach (var cart in ShopCartViewModel.ListCart)
            {
                cart.Price = PriceByQuantity(cart.Count, cart.Product.Price,
                    cart.Product.Price50, cart.Product.Price100);
                ShopCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShopCartViewModel);
           
        }
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity; // first get the identity
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //we need to populate all shop cart view model
            ShopCartViewModel.ListCart = _unitOW.ShopCart.GetAll(u => u.AppUserId == claim.Value, includeProperties: "Product"); // filter our records
            //When the order is placed we need to modify some details on order header
            ShopCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatusPending; // when the order is getting created, the status will be pending
            ShopCartViewModel.OrderHeader.OrderStatus = SD.StatusPending;
            ShopCartViewModel.OrderHeader.OrderDate = System.DateTime.Now;
            ShopCartViewModel.OrderHeader.AppUserId = claim.Value;
            
            // here we need to add get total
            foreach (var cart in ShopCartViewModel.ListCart)
            {
                cart.Price = PriceByQuantity(cart.Count, cart.Product.Price,
                    cart.Product.Price50, cart.Product.Price100);
                ShopCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            _unitOW.OrderHeader.Add(ShopCartViewModel.OrderHeader); //we have to add this to db, that is inside the viewmodel.orderHeader 
            _unitOW.Save();

            //here below we can create order details for all the items in the shopping cart
            foreach (var cart in ShopCartViewModel.ListCart)
            {
                OrderDetail orderDetail = new() // created an object of orderDetail
                                                //based on all the items in the card we need to populate the order detail
                {
                    ProductId = cart.ProductId,
                    OrderId = ShopCartViewModel.OrderHeader.Id, // we have already saved the changes so this param was getting from a data saved in db
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOW.OrderDetail.Add(orderDetail);
                _unitOW.Save(); // all order detail need to be added to db and saved
            }
            //after all of these we will have to clear our shopping card
            _unitOW.ShopCart.RemoveRange(ShopCartViewModel.ListCart);
            _unitOW.Save();

            return RedirectToAction("Index", "Home"); //redirect to index action of the home controller

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
            if (cart.Count <= 1)
            {
                _unitOW.ShopCart.Remove(cart);
            }
            else
            {
                _unitOW.ShopCart.DecrCounter(cart, 1); //increasing by one

            }
            _unitOW.Save(); // only if we invoke save method, it will be shown in database

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cart = _unitOW.ShopCart.GetFirstOrDefault(u => u.Id == cartId); // we need to retrieve from db
            _unitOW.ShopCart.Remove(cart); //increasing by one
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
