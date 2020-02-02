using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;

namespace MyShop.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private IRepository<Product> Context;
        private IRepository<ProductCategory> ProductCategories;

        public HomeController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoriesContext)
        {
            Context = productContext;
            ProductCategories = productCategoriesContext;
        }

        public ActionResult Index(string category = null)
        {
            var productCategories = ProductCategories.Collection().ToList();

            var products = category == null ? Context.Collection().ToList() : Context.Collection().Where(p => p.Category == category).ToList();

            var viewModel = new ProductListViewModel
            {
                Products = products,
                ProductCategories = productCategories
            };

            return View(viewModel);
        }

        public ActionResult Details(string id)
        {
            var product = Context.Find(id);
            if (product == null)
                return HttpNotFound();
            return View(product);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}