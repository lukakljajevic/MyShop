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
    [Authorize(Roles = "Admin")]
    public class ProductCategoryManagerController : Controller
    {
        private IRepository<ProductCategory> Context;

        public ProductCategoryManagerController(IRepository<ProductCategory> categoriesContext)
        {
            Context = categoriesContext;
        }

        public ActionResult Index()
        {
            var productCategories = Context.Collection().ToList();
            return View(productCategories);
        }

        public ActionResult Create() => View(new ProductCategory());

        [HttpPost]
        public ActionResult Create(ProductCategory pc)
        {
            if (!ModelState.IsValid)
                return View(pc);
            Context.Insert(pc);
            Context.Commit();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id)
        {
            var pc = Context.Find(id);
            if (pc == null)
                return HttpNotFound();
            return View(pc);
        }

        [HttpPost]
        public ActionResult Edit(ProductCategory pc, string id)
        {
            var pcToEdit = Context.Find(id);
            if (pcToEdit == null)
                return HttpNotFound();
            if (!ModelState.IsValid)
                return View(pc);

            pcToEdit.Category = pc.Category;

            Context.Commit();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            var pcToDelete = Context.Find(id);
            if (pcToDelete == null)
                return HttpNotFound();
            return View(pcToDelete);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string id)
        {
            var pcToDelete = Context.Find(id);
            if (pcToDelete == null)
                return HttpNotFound();
            Context.Delete(id);
            Context.Commit();
            return RedirectToAction("Index");
        }
    }
}