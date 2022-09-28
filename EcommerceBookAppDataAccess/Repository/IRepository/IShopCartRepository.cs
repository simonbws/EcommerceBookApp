using EcommerceBookApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookApp.DataAccess.Repository.IRepository
{
    public interface IShopCartRepository : IRepository<ShopCart>
    {
        int IncrCounter(ShopCart shopCart, int count);
        int DecrCounter(ShopCart shopCart, int count);
        
    }
}
