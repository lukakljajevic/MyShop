using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.DataAccess.InMemory;

namespace MyShop.WebUI.Controllers
{
    public class ProductCategoryManagerController : Controller
    {
        private IRepository<ProductCategory> context;

        public ProductCategoryManagerController(IRepository<ProductCategory> categoriesContext)
        {
            context = categoriesContext;
        }

        public ActionResult Index()
        {
            List<ProductCategory> productCategories = context.Collection().ToList();
            return View(productCategories);
        }

        public ActionResult Create()
        {
            ProductCategory pc = new ProductCategory();
            return View(pc);
        }

        [HttpPost]
        public ActionResult Create(ProductCategory pc)
        {
            if (!ModelState.IsValid)
                return View(pc);
            context.Insert(pc);
            context.Commit();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string Id)
        {
            ProductCategory pc = context.Find(Id);
            if (pc == null)
                return HttpNotFound();
            return View(pc);
        }

        [HttpPost]
        public ActionResult Edit(ProductCategory pc, string Id)
        {
            ProductCategory pcToEdit = context.Find(Id);
            if (pcToEdit == null)
                return HttpNotFound();
            if (!ModelState.IsValid)
                return View(pc);

            pcToEdit.Category = pc.Category;

            context.Commit();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string Id)
        {
            ProductCategory pcToDelete = context.Find(Id);
            if (pcToDelete == null)
                return HttpNotFound();
            return View(pcToDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            ProductCategory pcToDelete = context.Find(Id);
            if (pcToDelete == null)
                return HttpNotFound();
            context.Delete(Id);
            context.Commit();
            return RedirectToAction("Index");
        }
    }
}