using EcommerceBookApp.DataAccess.Repository.IRepository;
using EcommerceBookApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookApp.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private AppDbContext _db;
        public OrderHeaderRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string paymentStatus = null)
        {
            var orderDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderDb != null)
            {
                orderDb.OrderStatus = orderStatus;
                if (paymentStatus != null)
                {
                    orderDb.PaymentStatus = paymentStatus;
                }
            }
        }
    }
}
