using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using MyShop.Core.Models;

namespace MyShop.DataAccess.InMemory
{
    public class ProductCategoryRepository
    {
        private ObjectCache cache = MemoryCache.Default;
        private List<ProductCategory> productCategories;

        public ProductCategoryRepository()
        {
            productCategories = cache["productCategories"] as List<ProductCategory> ?? new List<ProductCategory>();
        }

        public void Commit()
        {
            cache["productCategories"] = productCategories;
        }

        public void Insert(ProductCategory pc)
        {
            productCategories.Add(pc);
        }

        public void Update(ProductCategory pc)
        {
            var productCategoryToUpdate = productCategories.Find(p => p.Id == pc.Id);
            if (productCategoryToUpdate != null)
                productCategoryToUpdate = pc;
            else
                throw new Exception("Product category not found");
        }

        public ProductCategory Find(string id)
        {
            var productCategory = productCategories.Find(p => p.Id == id);
            if (productCategory != null)
                return productCategory;
            throw new Exception("Product category not found");
        }

        public IQueryable<ProductCategory> Collection()
        {
            return productCategories.AsQueryable();
        }

        public void Delete(string id)
        {
            var productCategoryToRemove = productCategories.Find(p => p.Id == id);
            if (productCategoryToRemove != null)
                productCategories.Remove(productCategoryToRemove);
            else
                throw new Exception("Product category not found");
        }
    }
}
