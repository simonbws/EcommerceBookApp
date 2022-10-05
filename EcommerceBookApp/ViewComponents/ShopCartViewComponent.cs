using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceBookAppWeb.ViewComponents
{
    public class ShopCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShopCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //first we check if user is log in or not using the claims identity
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
                else
                {
                    //if session is null, we neet to go database and retrieve the number of shop carts for the log in user and assign that to SD.SessionCart
                    //we want to set our session cart instead of get by:
                    HttpContext.Session.SetInt32(SD.SessionCart,
                        _unitOfWork.ShopCart.GetAll(u => u.AppUserId == claim.Value).ToList().Count);
                    //we were using GetAll instead of GetFirstOrDefault, as FFoD will just return one
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
            }
            // if claim is null
            else
            {
                HttpContext.Session.Clear();
                return View(0);

            }
        }
    }
}
