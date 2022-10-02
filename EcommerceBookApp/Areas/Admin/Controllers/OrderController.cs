using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using EcommerceBookApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Claims;

namespace EcommerceBookAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // order controller should be only accessible by the authorized user
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOW;

        public OrderController(IUnitOfWork unitOW)
        {
            _unitOW = unitOW;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee)) //admin or employee can see all the order in order to process them
            {
                orderHeaders = _unitOW.OrderHeader.GetAll(includeProperties: "AppUser");
            }
            else // below they should only see the orders that belong to user id
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity; // here we get the user id of the log in user using claims
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOW.OrderHeader.GetAll(u=>u.AppUserId == claim.Value, includeProperties: "AppUser");
            }
            
            //below is where we are retrievieng all the orders
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPaymenet);
                    break;
                case "inprogress":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProgress);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusDelivered);
                    break;
                case "accepted":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusAccepted);
                    break;
                default:        
                    break;
            }

            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
