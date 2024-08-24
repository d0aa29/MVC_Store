using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.ViewModel
{
    public class ShoppingCartvM
    {
       
            public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
            public double OrderTotal { get; set; }
        //    public OrderHeader OrderHeader { get; set; }

    }
}
