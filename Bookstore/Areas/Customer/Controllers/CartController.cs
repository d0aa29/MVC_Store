using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Store.DataAccess.Repository.IRepository;
using Store.Models.ViewModel;
using System.Security.Claims;
using Store.Models;
using Store.Utility;

namespace Mystore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
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
                ShoppingCartList = _unitOfWork.ShoppingCart
                                  .GetAll(u => u.ApplicationUserId == userid
                                    ,includeproperties: "Product"),
                OrderHeader = new()

            };
            foreach (var cart in ShoppingCartvM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuntity(cart);
                ShoppingCartvM.OrderHeader.OrderTotal += (cart.Price *cart.Count);
            }
                return View(ShoppingCartvM);
        }
        public IActionResult plus(int cartId) { 
         
             var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
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
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var userid = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartvM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart
                                  .GetAll(u => u.ApplicationUserId == userid
                                    , includeproperties: "Product"),
                OrderHeader = new()

            };
            ShoppingCartvM.OrderHeader.ApplicationUser= _unitOfWork.appUser.Get(u=>u.Id == userid);

            ShoppingCartvM.OrderHeader.Name = ShoppingCartvM.OrderHeader.ApplicationUser.Name;
            ShoppingCartvM.OrderHeader.City = ShoppingCartvM.OrderHeader.ApplicationUser.City;
            ShoppingCartvM.OrderHeader.State = ShoppingCartvM.OrderHeader.ApplicationUser.State;
            ShoppingCartvM.OrderHeader.StreetAddress = ShoppingCartvM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartvM.OrderHeader.PhoneNumber = ShoppingCartvM.OrderHeader.ApplicationUser.PhoneNumber;


            foreach (var cart in ShoppingCartvM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuntity(cart);
                ShoppingCartvM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartvM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var userid = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartvM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userid
                                    , includeproperties: "Product");

            ShoppingCartvM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartvM.OrderHeader.ApplicationUserId = userid;
            AppUser applicationUser = _unitOfWork.appUser.Get(u => u.Id == userid);
            foreach (var cart in ShoppingCartvM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuntity(cart);
                ShoppingCartvM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // customer
                ShoppingCartvM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartvM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //company
                ShoppingCartvM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartvM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _unitOfWork.orderHeader.Add(ShoppingCartvM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartvM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartvM.OrderHeader.Id,
                    Count = cart.Count,
                    Price = cart.Price
                };
                _unitOfWork.orderDetail.Add(orderDetail);
               
            }
            _unitOfWork.Save();
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // customer and we need to capture payment

            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartvM.OrderHeader.Id });
        }

        public ActionResult OrderConfirmation(int id)
        {
            return View(id);
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
