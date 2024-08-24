﻿
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
// @using Mystore.Models 
using Store.Models;
using System.Diagnostics;
using Store.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        public IActionResult Index()
        {
            IEnumerable<Product> ProductList = _UnitOfWork.Product.GetAll(includeproperties: "Category");

            return View(ProductList);
        }
		public IActionResult Details(int proid)
		{
            ShoppingCart Cart = new()
            {

                Product = _UnitOfWork.Product.Get(u => u.Id == proid, includeproperties: "Category"),
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
            }
            else
            {
                _UnitOfWork.ShoppingCart.Add(Cart);
            }

            TempData["Succes"] = "Cart Updated Successfully";
            _UnitOfWork.Save();
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