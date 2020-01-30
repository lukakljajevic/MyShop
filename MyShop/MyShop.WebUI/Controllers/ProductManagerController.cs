using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.DataAccess.InMemory;

namespace MyShop.WebUI.Controllers
{
    public class ProductManagerController : Controller
    {
        private IRepository<Product> context;
        private IRepository<ProductCategory> productCategories;

        public ProductManagerController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoriesContext)
        {
            context = productContext;
            productCategories = productCategoriesContext;
        }

        public ActionResult Index()
        {
            List<Product> products = context.Collection().ToList();
            return View(products);
        }

        public ActionResult Create()
        {
            ProductManagerViewModel viewModel = new ProductManagerViewModel();
            viewModel.Product = new Product();
            viewModel.ProductCategories = productCategories.Collection();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(Product product)
        {

            if (!ModelState.IsValid)
            {
                ProductManagerViewModel viewModel = new ProductManagerViewModel();
                viewModel.Product = product;
                return View(viewModel);
            }
                
            context.Insert(product);
            context.Commit();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string Id)
        {
            ProductManagerViewModel viewModel = new ProductManagerViewModel();
            viewModel.Product = context.Find(Id);
            viewModel.ProductCategories = productCategories.Collection();

            if (viewModel.Product == null)
                return HttpNotFound();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(Product product, string Id)
        {
            Product productToEdit = context.Find(Id);
            if (productToEdit == null)
                return HttpNotFound();
            if (!ModelState.IsValid)
                return View(product);

            productToEdit.Name = product.Name;
            productToEdit.Category = product.Category;
            productToEdit.Price = product.Price;
            productToEdit.Description = product.Description;
            productToEdit.Image = product.Image;

            context.Commit();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string Id)
        {
            Product productToDelete = context.Find(Id);
            if (productToDelete == null)
                return HttpNotFound();
            return View(productToDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            Product productToDelete = context.Find(Id);
            if (productToDelete == null)
                return HttpNotFound();
            context.Delete(Id);
            context.Commit();
            return RedirectToAction("Index");
        }
    }
}