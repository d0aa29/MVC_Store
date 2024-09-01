using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
namespace Store.Models.ViewModel
{
	public class ProductVM
	{
       public Product Product { get; set; }
		public IEnumerable<Product> ProductList { get; set; }

		[ValidateNever]
		public IEnumerable<SelectListItem> mycategorylist { get; set; }
		
	}
}
