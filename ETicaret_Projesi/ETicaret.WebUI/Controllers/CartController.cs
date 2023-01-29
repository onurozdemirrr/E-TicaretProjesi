using ETicaret.BusinessLayer.Abstract;
using ETicaret.WebUI.Identity;
using ETicaret.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System;
using Iyzipay.Model;
using Iyzipay;
using Iyzipay.Request;
using Newtonsoft.Json;
using ETicaret.BusinessLayer.Concrete;
using ETicaret.Entities;

namespace ETicaret.WebUI.Controllers
{
    public class CartController : Controller
    {
        private ICartService _cartService;
        private UserManager<User> _userManager;
        private IOrderService _orderService;
        public CartController(ICartService cartService, UserManager<User> userManager, IOrderService orderService)
        {
            _cartService = cartService;
            _userManager = userManager;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            if (_cartService.GetCardByUserId(userId) == null)
            {
                _cartService.InitializerCart(userId);
            }

            var cart = _cartService.GetCardByUserId(_userManager.GetUserId(User));
            var model = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(x =>
                new CartItemModel()
                {
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    Quantity = x.Quantity,
                    Price = x.Product.Price,
                    ImageUrl = x.Product.ImageUrl
                }
                ).ToList()
            };
            return View(model);
        }

        public IActionResult CompleteShopping()
        {
            var cart = _cartService.GetCardByUserId(_userManager.GetUserId(User));
            OrderModel model = new OrderModel();
            model.CartModel = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(x => new CartItemModel()
                {
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    Quantity = x.Quantity,
                    Price = x.Product.Price,
                    ImageUrl = x.Product.ImageUrl,

                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CompleteShopping(OrderModel model)
        {
            ModelState.Remove("CartModel");
            ModelState.Remove("Phone");
            ModelState.Remove("Note");
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var cart = _cartService.GetCardByUserId(userId);

                model.CartModel = new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(i => new CartItemModel()
                    {
                        CartItemId = i.Id,
                        ProductName = i.Product.Name,
                        Price = i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                };

                var payment = PaymentProcess(model);

                if (payment.Status == "success")
                {
                    SaveOrder(model, payment, userId);
                    ClearCart(model.CartModel.CartId);
                    return View("Success");
                }
                else
                {
                    var msg = new AlertMessage()
                    {
                        Message = $"{payment.ErrorMessage}",
                        AlertType = "danger"
                    };

                    TempData["message"] = JsonConvert.SerializeObject(msg);
                }
            }
            return View(model);

        }

        private void SaveOrder(OrderModel model, Payment payment, string userId)
        {
            // Sepete eklendikten sonra ve ödeme işlemi tamamlandıktan sonra Cart/CartItems içindeki bilgiler Order/OrderItems tablosuna aktarılacak.
            Order order = new Order();

            // OrderNumber için benzersiz sayı üretmemiz gerekiyor. Bunu da Random ile yapacağım.
            // Normalde Herbir sipariş için uniq olan bir order number oluşturabiliriz ve bu numarayı da OrderNumber'a verebiliriz.
            order.OrderNumber = new Random().Next(111111, 999999).ToString();
            order.OrderState = EnumOrderState.completed;
            order.PaymentType = EnumPaymentType.CreditCart;
            order.PaymentId = payment.PaymentId;
            order.ConversationId = payment.ConversationId;
            order.OrderDate = DateTime.Now;
            order.FirstName = model.FirstName;
            order.LastName = model.LastName;
            order.UserId = userId;
            order.Adress = model.Adress;
            order.City = model.City;
            order.Note = "";
            order.Phone = "";
            order.Email = "";

            order.OrderItems = new List<ETicaret.Entities.OrderItem>();
            foreach (var item in model.CartModel.CartItems)
            {
                var orderItem = new ETicaret.Entities.OrderItem()
                {
                    Price = (double)item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId,
                };
                order.OrderItems.Add(orderItem);
            }
            _orderService.Create(order);
        }
        private void ClearCart(int cartId)
        {
            _cartService.ClearCart(cartId);
        }


        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                _cartService.AddToCart(userId, productId, quantity);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.DeleteFromCart(userId, productId);
            return RedirectToAction("Index");
        }

        private Payment PaymentProcess(OrderModel model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-BS8Rj2pBXNWBwLQPkdHAxRPWT51l3JLE";
            options.SecretKey = "sandbox-kwzQppqjgoNFfyXT9ccQp0339VcMJlo3";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(111111111, 999999999).ToString();
            request.Price = model.CartModel.TotalPrice().ToString();
            request.PaidPrice = model.CartModel.TotalPrice().ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;
            paymentCard.ExpireYear = model.ExpirationYear;
            paymentCard.Cvc = model.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = "John";
            buyer.Surname = "Doe";
            buyer.GsmNumber = "+905350000000";
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            foreach (var item in model.CartModel.CartItems)
            {
                BasketItem firstBasketItem = new BasketItem();
                firstBasketItem.Id = item.ProductId.ToString();
                firstBasketItem.Name = item.ProductName;
                firstBasketItem.Category1 = item.ProductName;
                firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                firstBasketItem.Price = item.Price.ToString();
                basketItems.Add(firstBasketItem);
            }
            request.BasketItems = basketItems;

            Payment payment = Payment.Create(request, options);
            return payment;
        }

        public IActionResult GetOrders()
        {
            var userId = _userManager.GetUserId(User);
            var orders = _orderService.GetOrders(userId);
            // Veritabanından ilgili bilgileri aldık. Bunları ekranda göstermek için OrderListModel'i kullanacağım.
            var orderListModel = new List<OrderListModel>();
            OrderListModel orderModel;
            foreach (var order in orders)
            {
                orderModel = new OrderListModel();
                orderModel.OrderId = order.Id;
                orderModel.OrderNumber = order.OrderNumber;
                orderModel.UserId = order.UserId;
                orderModel.Adress = order.Adress;
                orderModel.Phone = order.Phone;
                orderModel.FirstName = order.FirstName;
                orderModel.LastName = order.LastName;
                orderModel.Email = order.Email;
                orderModel.City = order.City;
                orderModel.OrderState = order.OrderState;
                orderModel.PaymentType = order.PaymentType;

                orderModel.OrderItems = order.OrderItems.Select(x => new OrderItemModel()
                {
                    OrderItemId = x.Id,
                    Name = x.Product.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    ImageUrl = x.Product.ImageUrl
                }).ToList();

                orderListModel.Add(orderModel);
            }

            return View("MyOrders", orderListModel);
        }
    }
}
