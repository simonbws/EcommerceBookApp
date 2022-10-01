using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
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
        public IActionResult GetAll()
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = _unitOW.OrderHeader.GetAll(includeProperties: "AppUser");
            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
