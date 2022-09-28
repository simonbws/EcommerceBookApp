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
    public class ShopCartRepository : Repository<ShopCart>, IShopCartRepository
    {
        private AppDbContext _db;
        public ShopCartRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public int DecrCounter(ShopCart shopCart, int count)
        {
            shopCart.Count -= count;
            return shopCart.Count;
        }

        public int IncrCounter(ShopCart shopCart, int count)
        {
            shopCart.Count+= count;
            return shopCart.Count;
        }
    }
}
