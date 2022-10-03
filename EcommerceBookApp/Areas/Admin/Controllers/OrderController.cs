using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using EcommerceBookApp.Models.ViewModels;
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
        [BindProperty]
        public OrderViewModel OrderViewModel { get; set; }

        public OrderController(IUnitOfWork unitOW)
        {
            _unitOW = unitOW;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId) // parameter is taken from order js,routing Details?orderId, now we add details view
        {
            OrderViewModel = new OrderViewModel()
            {
                OrderHeader = _unitOW.OrderHeader.GetFirstOrDefault(u=>u.Id == orderId, includeProperties: "AppUser"),
                OrderDetail = _unitOW.OrderDetail.GetAll(u=>u.OrderId == orderId, includeProperties: "Product"),
            };
            return View(OrderViewModel); // here is get action for details, details is in order js
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderProperties() 
        {
            var orderHeaderFromDatabase = _unitOW.OrderHeader.GetFirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id, tracked:false);
            orderHeaderFromDatabase.Name = OrderViewModel.OrderHeader.Name;
            orderHeaderFromDatabase.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
            orderHeaderFromDatabase.StreetAddress = OrderViewModel.OrderHeader.StreetAddress;
            orderHeaderFromDatabase.City = OrderViewModel.OrderHeader.City;
            orderHeaderFromDatabase.State = OrderViewModel.OrderHeader.State;
            orderHeaderFromDatabase.PostalCode = OrderViewModel.OrderHeader.PostalCode;
            if (OrderViewModel.OrderHeader.Carrier != null)
            {
                orderHeaderFromDatabase.Carrier = OrderViewModel.OrderHeader.Carrier;
            }
            if (OrderViewModel.OrderHeader.TrackNumber != null)
            {
                orderHeaderFromDatabase.TrackNumber = OrderViewModel.OrderHeader.TrackNumber;
            }
            //_unitOW.OrderHeader.Update(orderHeaderFromDatabase);
            _unitOW.Save();
            TempData["Success"] = "Order Properties has been updated successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderHeaderFromDatabase.Id });
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
