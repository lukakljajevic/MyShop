using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;

namespace MyShop.WebUI.Controllers
{
    public class BasketController : Controller
    {
        private IBasketService BasketService;

        public BasketController(IBasketService basketService)
        {
            BasketService = basketService;
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
    }
}