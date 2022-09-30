using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookApp.Models.ViewModels
{
	public class ShopCartViewModel
	{
		public IEnumerable<ShopCart> ListCart { get; set; }
		
		public OrderHeader OrderHeader { get; set; }
	}
}
