using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Store.DataAccess.Repository.IRepository;
using Store.Models.ViewModel;
using System.Security.Claims;
using Store.Models;
using Store.Utility;
using Stripe.Checkout;

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
                cart.Price = GetPriceBasedOnQuantity(cart);
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
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartvM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeproperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartvM.OrderHeader.ApplicationUser = _unitOfWork.appUser.Get(u => u.Id == userId);

            ShoppingCartvM.OrderHeader.Name = ShoppingCartvM.OrderHeader.ApplicationUser.Name;
            ShoppingCartvM.OrderHeader.PhoneNumber = ShoppingCartvM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartvM.OrderHeader.StreetAddress = ShoppingCartvM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartvM.OrderHeader.City = ShoppingCartvM.OrderHeader.ApplicationUser.City;
            ShoppingCartvM.OrderHeader.State = ShoppingCartvM.OrderHeader.ApplicationUser.State;
            ShoppingCartvM.OrderHeader.PostalCode = ShoppingCartvM.OrderHeader.ApplicationUser.PostalCode;



            foreach (var cart in ShoppingCartvM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartvM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartvM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartvM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeproperties: "Product");

            ShoppingCartvM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartvM.OrderHeader.ApplicationUserId = userId;

            AppUser applicationUser = _unitOfWork.appUser.Get(u => u.Id == userId);


            foreach (var cart in ShoppingCartvM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartvM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //it is a regular customer 
                ShoppingCartvM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartvM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //it is a company user
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
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.orderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // Customer and we need to capture payment
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartvM.OrderHeader.Id}",
                    CancelUrl = domain + $"Customer/Cart/index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartvM.ShoppingCartList)
                {
                    var sessionLineItem = new Stripe.Checkout.SessionLineItemOptions
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new Stripe.Checkout.SessionService();
                Stripe.Checkout.Session session = service.Create(options);

                if (session != null)
                {
                    _unitOfWork.orderHeader.UpdateStripePaymentId(ShoppingCartvM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);
                }
                else
                {
                    // Handle error for failed session creation
                    throw new Exception("Failed to create Stripe session.");
                }
            }


            //if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            //{
            //    //it is a regular customer account and we need to capture payment
            //    //stripe logic
            //    var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            //    var options = new SessionCreateOptions
            //    {
            //        SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartvM.OrderHeader.Id}",
            //        CancelUrl = domain + "customer/cart/index",
            //        LineItems = new List<SessionLineItemOptions>(),
            //        Mode = "payment",
            //    };

            //    foreach (var item in ShoppingCartvM.ShoppingCartList)
            //    {
            //        var sessionLineItem = new SessionLineItemOptions
            //        {
            //            PriceData = new SessionLineItemPriceDataOptions
            //            {
            //                UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
            //                Currency = "usd",
            //                ProductData = new SessionLineItemPriceDataProductDataOptions
            //                {
            //                    Name = item.Product.Title
            //                }
            //            },
            //            Quantity = item.Count
            //        };
            //        options.LineItems.Add(sessionLineItem);
            //    }


            //    var service = new SessionService();
            //    Session session = service.Create(options);
            //    _unitOfWork.orderHeader.UpdateStripePaymentId(ShoppingCartvM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            //    _unitOfWork.Save();
            //    Response.Headers.Add("Location", session.Url);
            //    return new StatusCodeResult(303);

            //}

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartvM.OrderHeader.Id });
        }




        //public IActionResult Summary() {
        //    var claimsidentity = (ClaimsIdentity)User.Identity;
        //    var userid = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    ShoppingCartvM = new()
        //    {
        //        ShoppingCartList = _unitOfWork.ShoppingCart
        //                          .GetAll(u => u.ApplicationUserId == userid
        //                            , includeproperties: "Product"),
        //        OrderHeader = new()

        //    };
        //    ShoppingCartvM.OrderHeader.ApplicationUser= _unitOfWork.appUser.Get(u=>u.Id == userid);

        //    ShoppingCartvM.OrderHeader.Name = ShoppingCartvM.OrderHeader.ApplicationUser.Name;
        //    ShoppingCartvM.OrderHeader.City = ShoppingCartvM.OrderHeader.ApplicationUser.City;
        //    ShoppingCartvM.OrderHeader.State = ShoppingCartvM.OrderHeader.ApplicationUser.State;
        //    ShoppingCartvM.OrderHeader.StreetAddress = ShoppingCartvM.OrderHeader.ApplicationUser.StreetAddress;
        //    ShoppingCartvM.OrderHeader.PhoneNumber = ShoppingCartvM.OrderHeader.ApplicationUser.PhoneNumber;


        //    foreach (var cart in ShoppingCartvM.ShoppingCartList)
        //    {
        //        cart.Price = GetPriceBasedOnQuntity(cart);
        //        ShoppingCartvM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        //    }
        //    return View(ShoppingCartvM);
        //}

        //[HttpPost]
        //[ActionName("Summary")]
        //public IActionResult SummaryPost()
        //{
        //    var claimsidentity = (ClaimsIdentity)User.Identity;
        //    var userid = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    ShoppingCartvM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userid
        //                            , includeproperties: "Product");

        //    ShoppingCartvM.OrderHeader.OrderDate = DateTime.Now;
        //    ShoppingCartvM.OrderHeader.ApplicationUserId = userid;
        //    AppUser applicationUser = _unitOfWork.appUser.Get(u => u.Id == userid);
        //    foreach (var cart in ShoppingCartvM.ShoppingCartList)
        //    {
        //        cart.Price = GetPriceBasedOnQuntity(cart);
        //        ShoppingCartvM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
        //    }
        //    if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        //    {
        //        // customer
        //        ShoppingCartvM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        //        ShoppingCartvM.OrderHeader.OrderStatus = SD.StatusPending;
        //    }
        //    else
        //    {
        //        //company
        //        ShoppingCartvM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
        //        ShoppingCartvM.OrderHeader.OrderStatus = SD.StatusApproved;
        //    }
        //    _unitOfWork.orderHeader.Add(ShoppingCartvM.OrderHeader);
        //    _unitOfWork.Save();

        //    foreach (var cart in ShoppingCartvM.ShoppingCartList)
        //    {
        //        OrderDetail orderDetail = new()
        //        {
        //            ProductId = cart.ProductId,
        //            OrderHeaderId = ShoppingCartvM.OrderHeader.Id,
        //            Count = cart.Count,
        //            Price = cart.Price
        //        };
        //        _unitOfWork.orderDetail.Add(orderDetail);

        //    }
        //    _unitOfWork.Save();

        //    if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        //    {
        //        // customer and we need to capture payment
        //        var domain = Request.Scheme + "://" + Request.Host.Value + "/";
        //        var options = new Stripe.Checkout.SessionCreateOptions
        //        {

        //            SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartvM.OrderHeader.Id}",
        //            CancelUrl = domain + $"Customer/Cart/index",
        //            LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
        //            Mode = "payment",
        //        };
        //        foreach (var item in ShoppingCartvM.ShoppingCartList)
        //        {
        //            var sessionLineItem = new SessionLineItemOptions
        //            {
        //                PriceData = new SessionLineItemPriceDataOptions
        //                {
        //                    UnitAmount = (long)(item.Price * 100),
        //                    Currency = "usd",
        //                    ProductData = new SessionLineItemPriceDataProductDataOptions
        //                    {
        //                        Name = item.Product.Title
        //                    }
        //                },

        //                Quantity = item.Count

        //            };
        //            options.LineItems.Add(sessionLineItem);
        //        }
        //        var service = new SessionService();
        //        Session session = service.Create(options);
        //        _unitOfWork.orderHeader.UpdateStripePaymentId(ShoppingCartvM.OrderHeader.Id, session.Id, session.PaymentIntentId);
        //        _unitOfWork.Save();
        //        Response.Headers.Add("Location", session.Url);
        //        return new StatusCodeResult(303);
        //    }
        //    return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartvM.OrderHeader.Id });
        //}

        public ActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == id, includeproperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is an order by customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.orderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.orderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                HttpContext.Session.Clear();

            }

            //_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Bulky Book",
            //    $"<p>New Order Created - {orderHeader.Id}</p>");

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
        }

        public double GetPriceBasedOnQuantity(ShoppingCart ShoppingCart) {
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
