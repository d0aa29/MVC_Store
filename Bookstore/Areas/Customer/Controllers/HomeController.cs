
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.NetworkInformation;
// @using Mystore.Models 
using Store.Models;
using System.Diagnostics;
using Store.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Store.Utility;
using Microsoft.AspNetCore.Http;
using Store.Models.ViewModel;

namespace Mystore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUnitOfWork _UnitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork UnitOfWork)
        {

            _UnitOfWork = UnitOfWork;
            _logger = logger;
        }
		//		public IActionResult Index(int? category)
		//{

		//    // Get all categories from the repository
		//    var categories = _UnitOfWork.Category.GetAll();

		//    // Create a new ProductVM instance
		//    var viewModel = new ProductVM
		//    {
		//        Product = new Product(),  // Initialize the Product object

		//        // Populate the category list from the database
		//        mycategorylist = categories.Select(c => new SelectListItem
		//        {
		//            Text = c.Name,       // Display category name
		//            Value = c.Id.ToString()  // Value sent back to the server
		//        }).ToList()
		//    };

		//    // Set the default selected category if provided
		//    if (category.HasValue)
		//    {
		//        viewModel.Product.CategoryId = category.Value;
		//    }

		//    // Fetch products based on the selected category
		//       viewModel.ProductList = category.HasValue
		//        ? _UnitOfWork.Product.GetAll(p => p.CategoryId == category.Value, includeproperties: "Category,ProductImages")
		//        : _UnitOfWork.Product.GetAll(includeproperties: "Category,ProductImages");

		//    return View(viewModel);
		//}
		//public IActionResult Index(int? category)
		//{
		//	// Get all categories from the repository
		//	var categories = _UnitOfWork.Category.GetAll();

		//	// Create a new ProductVM instance
		//	var viewModel = new ProductVM
		//	{
		//		Product = new Product(),  // Initialize the Product object

		//		// Populate the category list from the database
		//		mycategorylist = categories.Select(c => new SelectListItem
		//		{
		//			Text = c.Name,       // Display category name
		//			Value = c.Id.ToString()  // Value sent back to the server
		//		}).ToList()
		//	};

		//	// Fetch products based on the selected category
		//	viewModel.ProductList = category.HasValue
		//		? _UnitOfWork.Product.GetAll(p => p.CategoryId == category.Value, includeproperties: "Category,ProductImages")
		//		: _UnitOfWork.Product.GetAll(includeproperties: "Category,ProductImages");

		//	return View(viewModel);
		//}




		public IActionResult Index(int? category)
		{
			var categories = _UnitOfWork.Category.GetAll();

			// Prepare the view model
			var viewModel = new ProductVM
			{
				Product = new Product(),
				mycategorylist = categories.Select(c => new SelectListItem
				{
					Text = c.Name,
					Value = c.Id.ToString()
				}).ToList()
			};

			// Set the selected category
			ViewData["SelectedCategory"] = category.HasValue ? category.Value.ToString() : "";

			// Fetch products based on the selected category
			if (category == -1)
			{
				viewModel.ProductList = _UnitOfWork.Product.GetAll(includeproperties: "Category,ProductImages");
			}
			else
			{
				viewModel.ProductList = category.HasValue
					? _UnitOfWork.Product.GetAll(p => p.CategoryId == category.Value, includeproperties: "Category,ProductImages")
					: _UnitOfWork.Product.GetAll(includeproperties: "Category,ProductImages");
			}
			return View(viewModel);
		}


		//public IActionResult Index(int? category)
		//{
		//	// Debug and log the category parameter
		//	// e.g., _logger.LogInformation("Category: " + category);

		//	// Get all categories from the repository
		//	var categories = _UnitOfWork.Category.GetAll();

		//	// Create a new ProductVM instance
		//	var viewModel = new ProductVM
		//	{
		//		Product = new Product(),  // Initialize the Product object

		//		// Populate the category list from the database
		//		mycategorylist = categories.Select(c => new SelectListItem
		//		{
		//			Text = c.Name,       // Display category name
		//			Value = c.Id.ToString(),  // Value sent back to the server
		//			Selected = c.Id == category // Set selected if it matches the category
		//		}).ToList()
		//	};

		//	// Fetch products based on the selected category
		//	viewModel.ProductList = category.HasValue
		//		? _UnitOfWork.Product.GetAll(p => p.CategoryId == category.Value, includeproperties: "Category,ProductImages")
		//		: _UnitOfWork.Product.GetAll(includeproperties: "Category,ProductImages");

		//	return View(viewModel);
		//}



		//public IActionResult Index()
		//{
		//    IEnumerable<Product> ProductList = _UnitOfWork.Product.GetAll(includeproperties: "Category,ProductImages");

		//    return View(ProductList);
		//}
		public IActionResult Details(int proid)
		{
            ShoppingCart Cart = new()
            {

                Product = _UnitOfWork.Product.Get(u => u.Id == proid, includeproperties: "Category,ProductImages"),
                Count = 1,
                ProductId = proid
            };
			return View(Cart);
		}
         
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart Cart)
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var userid = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            Cart.ApplicationUserId = userid;
          //  TempData["Success"] = "Cart Updated Successfully";
            ShoppingCart cartFromDb = _UnitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId==userid
            && u.ProductId== Cart.ProductId);

            if (cartFromDb != null)
            {
                cartFromDb.Count += Cart.Count;
                _UnitOfWork.ShoppingCart.Update(cartFromDb);
				_UnitOfWork.Save();
			}
            else
            {
                _UnitOfWork.ShoppingCart.Add(Cart);
				_UnitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, 
                    _UnitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userid).Count());
			}

            TempData["Succes"] = "Cart Updated Successfully";
           
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}