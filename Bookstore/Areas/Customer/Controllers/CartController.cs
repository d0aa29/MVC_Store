﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Store.DataAccess.Repository.IRepository;
using Store.Models.ViewModel;
using System.Security.Claims;
using Store.Models;

namespace Mystore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartvM ShoppingCartvM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var userid = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartvM = new() {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userid
                ,includeproperties: "Product")
            }
            ;
            foreach (var cart in ShoppingCartvM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuntity(cart);
                ShoppingCartvM.OrderTotal += (cart.Price *cart.Count);
            }
                return View(ShoppingCartvM);
        }
        public IActionResult plus(int cartid) { 
            var cartFromDb= _unitOfWork.ShoppingCart.Get(u=>u.Id==cartid);
            cartFromDb.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int cartid)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartid);
            if (cartFromDb.Count <= 1){
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else{          
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartid)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartid);            
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
             
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary() { 
         return View();
        }

        public double GetPriceBasedOnQuntity(ShoppingCart ShoppingCart) {
            if (ShoppingCart.Count<=50)
                return ShoppingCart.Product.Price;
            else 
            {
                if (ShoppingCart.Count <= 100)
                    return ShoppingCart.Product.Price50;
                else 
                    return ShoppingCart.Product.Price100;
            }
            
        
        }
    }
}