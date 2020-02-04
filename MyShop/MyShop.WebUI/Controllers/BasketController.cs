using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;
using MyShop.Core.Models;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        private IBasketService BasketService;
        private IOrderService OrderService { get; set; }

        public BasketController(IBasketService basketService, IOrderService orderService)
        {
            BasketService = basketService;
            OrderService = orderService;
        }

        public ActionResult Index()
        {
            var model = BasketService.GetBasketItems(HttpContext);
            return View(model);
        }

        public ActionResult AddToBasket(string productId)
        {
            BasketService.AddToBasket(HttpContext, productId);
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromBasket(string id)
        {
            BasketService.RemoveFromBasket(HttpContext, id);
            return RedirectToAction("Index");
        }

        public PartialViewResult BasketSummary()
        {
            var basketSummary = BasketService.GetBasketSummary(HttpContext);
            return PartialView(basketSummary);
        }

        public ActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Checkout(Order order)
        {
            var basketItems = BasketService.GetBasketItems(HttpContext);
            order.OrderStatus = "Order Created";

            // process payment

            order.OrderStatus = "Payment processed";
            OrderService.CreateOrder(order, basketItems);
            BasketService.ClearBasket(HttpContext);
            return RedirectToAction("Thankyou", new {orderId = order.Id});
        }

        public ActionResult ThankYou(string orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

    }
}