using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MyShop.Core.Contracts;
using MyShop.Core.Models;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        private IBasketService BasketService;
        private IOrderService OrderService { get; set; }
        private IRepository<Customer> Customers;

        public BasketController(IBasketService basketService, IOrderService orderService, IRepository<Customer> customers)
        {
            BasketService = basketService;
            OrderService = orderService;
            Customers = customers;
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

        [Authorize]
        public ActionResult Checkout()
        {
            var customer = Customers.Collection().FirstOrDefault(c => c.Email == User.Identity.Name);
            if (customer == null) return RedirectToAction("Error");
            var order = new Order
            {
                Email = customer.Email,
                City = customer.City,
                State = customer.State,
                Street = customer.Street,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                ZipCode = customer.ZipCode
            };

            return View(order);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Checkout(Order order)
        {
            var basketItems = BasketService.GetBasketItems(HttpContext);
            order.OrderStatus = "Order Created";
            order.Email = User.Identity.Name;

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