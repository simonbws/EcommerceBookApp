using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using EcommerceBookApp.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EcommerceBookAppWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            orderHeaders = _unitOW.OrderHeader.GetAll(includeProperties: "AppUser");

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
