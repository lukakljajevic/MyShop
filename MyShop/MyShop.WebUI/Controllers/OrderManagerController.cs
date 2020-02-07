using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;
using MyShop.Core.Models;

namespace MyShop.WebUI.Controllers
{
    public class OrderManagerController : Controller
    {
        private IOrderService OrderService;

        public OrderManagerController(IOrderService orderService)
        {
            OrderService = orderService;
        }

        public ActionResult Index()
        {
            var orders = OrderService.GetOrderList();
            return View(orders);
        }

        public ActionResult UpdateOrder(string id)
        {
            ViewBag.StatusList = new List<string>
                {"Order Created", "Payment Processed", "Order Shipped", "Order Complete"};
            var order = OrderService.GetOrder(id);
            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateOrder(Order updateOrder)
        {
            var order = OrderService.GetOrder(updateOrder.Id);
            order.OrderStatus = updateOrder.OrderStatus;
            OrderService.UpdateOrder(order);
            return RedirectToAction("Index");
        }


    }
}