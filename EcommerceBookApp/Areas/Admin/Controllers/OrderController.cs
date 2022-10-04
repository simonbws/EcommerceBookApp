using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using EcommerceBookApp.Models.ViewModels;
using EcommerceBookApp.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Stripe;
using Stripe.Checkout;
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
                OrderHeader = _unitOW.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "AppUser"),
                OrderDetail = _unitOW.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product"),
            };
            return View(OrderViewModel); // here is get action for details, details is in order js
        }
        [ActionName("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPay() // parameter is taken from order js,routing Details?orderId, now we add details view
        {
            OrderViewModel.OrderHeader = _unitOW.OrderHeader.GetFirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id, includeProperties: "AppUser");
            OrderViewModel.OrderDetail = _unitOW.OrderDetail.GetAll(u => u.OrderId == OrderViewModel.OrderHeader.Id, includeProperties: "Product");

            //stripe settings
            var domain = "https://localhost:7101/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>()
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderid={OrderViewModel.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderViewModel.OrderHeader.Id}",
            };

            foreach (var item in OrderViewModel.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions

                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // 20.00 we need to convert to 2000, its double so we have to convert to long
                        Currency = "pln",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);

            }

            var service = new SessionService();
            Session session = service.Create(options); //here session is created
            _unitOW.OrderHeader.UpdateStripePayId(OrderViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOW.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303); 
             
        }

        public IActionResult PaymentConfirmation(int orderHeaderid)
        {
            OrderHeader orderHeader = _unitOW.OrderHeader.GetFirstOrDefault(u => u.Id == orderHeaderid);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPaymenet)
            {
                //then we want to create a new session service
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId); //here session is created 
                //Upper, we retrieve OH from DB based on the id, in order to check stripe status
                //now we want to check status to make sure if the payment is actually done
                //only then we will aprove the status
                if (session.PaymentStatus.ToLower() == "paid")
                {

                    _unitOW.OrderHeader.UpdateStatus(orderHeaderid, orderHeader.OrderStatus, SD.PaymentStatusAccepted);
                    _unitOW.Save();
                }
            }

            return View(orderHeaderid);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderProperties()
        {
            var orderHeaderFromDatabase = _unitOW.OrderHeader.GetFirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id, tracked: false);
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
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult BeginProcess()
        {
            _unitOW.OrderHeader.UpdateStatus(OrderViewModel.OrderHeader.Id, SD.StatusInProgress);
            _unitOW.Save();
            TempData["Success"] = "Order Properties has been updated successfully";
            return RedirectToAction("Details", "Order", new { orderId = OrderViewModel.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult DeliverOrder()
        {
            var orderHeader = _unitOW.OrderHeader.GetFirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id, tracked: false);
            orderHeader.TrackNumber = OrderViewModel.OrderHeader.TrackNumber;
            orderHeader.Carrier = OrderViewModel.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusDelivered;
            orderHeader.ShipDate = DateTime.Now;
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPaymenet)
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }
            _unitOW.OrderHeader.Update(orderHeader);
            _unitOW.Save();
            TempData["Success"] = "Order Deliver successfully! ";
            return RedirectToAction("Details", "Order", new { orderId = OrderViewModel.OrderHeader.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOW.OrderHeader.GetFirstOrDefault(u => u.Id == OrderViewModel.OrderHeader.Id, tracked: false);
            if (orderHeader.PaymentStatus == SD.PaymentStatusAccepted)
            {
                var options = new RefundCreateOptions()
                {
                    Reason=RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                    
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOW.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusReturned);
            }
            else
            {
                _unitOW.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);

            }
            _unitOW.Save();
            
            TempData["Success"] = "Order Cancelled successfully! ";
            return RedirectToAction("Details", "Order", new { orderId = OrderViewModel.OrderHeader.Id });
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
                orderHeaders = _unitOW.OrderHeader.GetAll(u => u.AppUserId == claim.Value, includeProperties: "AppUser");
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
