using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.DataAccess.InMemory;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductManagerController : Controller
    {
        private IRepository<Product> Context;
        private IRepository<ProductCategory> ProductCategories;

        public ProductManagerController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoriesContext)
        {
            Context = productContext;
            ProductCategories = productCategoriesContext;
        }

        public ActionResult Index()
        {
            var products = Context.Collection().ToList();
            return View(products);
        }

        public ActionResult Create()
        {
            var viewModel = new ProductManagerViewModel
            {
                Product = new Product(),
                ProductCategories = ProductCategories.Collection()
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(Product product, HttpPostedFileBase file)
        {

            if (!ModelState.IsValid)
            {
                var viewModel = new ProductManagerViewModel { Product = product };
                return View(viewModel);
            }

            if (file != null)
            {
                product.Image = product.Id + Path.GetExtension(file.FileName);
                file.SaveAs(Server.MapPath("/Content/ProductImages/") + product.Image);
            }
                
            Context.Insert(product);
            Context.Commit();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string Id)
        {
            var viewModel = new ProductManagerViewModel
            {
                Product = Context.Find(Id), 
                ProductCategories = ProductCategories.Collection()
            };

            if (viewModel.Product == null)
                return HttpNotFound();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(Product product, string id, HttpPostedFileBase file)
        {
            var productToEdit = Context.Find(id);
            if (productToEdit == null)
                return HttpNotFound();
            if (!ModelState.IsValid)
            {
                var viewModel = new ProductManagerViewModel {Product = product};
                return View(viewModel);
            }

            if (file != null)
            {
                productToEdit.Image = product.Id + Path.GetExtension(file.FileName);
                file.SaveAs(Server.MapPath("/Content/ProductImages/") + productToEdit.Image);
            }

            productToEdit.Name = product.Name;
            productToEdit.Category = product.Category;
            productToEdit.Price = product.Price;
            productToEdit.Description = product.Description;

            Context.Commit();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string Id)
        {
            var productToDelete = Context.Find(Id);
            if (productToDelete == null)
                return HttpNotFound();
            return View(productToDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            var productToDelete = Context.Find(Id);
            if (productToDelete == null)
                return HttpNotFound();
            Context.Delete(Id);
            Context.Commit();
            return RedirectToAction("Index");
        }
    }
}