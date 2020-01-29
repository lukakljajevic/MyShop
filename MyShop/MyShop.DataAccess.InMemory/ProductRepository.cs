﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using MyShop.Core;
using MyShop.Core.Models;

namespace MyShop.DataAccess.InMemory
{
    class ProductRepository
    {
        private ObjectCache cache = MemoryCache.Default;
        private List<Product> products;

        public ProductRepository()
        {
            products = cache["products"] as List<Product> ?? new List<Product>();
        }

        public void Commit()
        {
            cache["products"] = products;
        }

        public void Insert(Product p)
        {
            products.Add(p);
        }

        public void Update(Product product)
        {
            var productToUpdate = products.Find(p => p.Id == product.Id);
            if (productToUpdate != null)
                productToUpdate = product;
            else
                throw new Exception("Product not found");
        }

        public Product Find(string Id)
        {
            var product = products.Find(p => p.Id == Id);
            if (product != null)
                return product;
            throw new Exception("Product not found");
        }

        public IQueryable<Product> Collection()
        {
            return products.AsQueryable();
        }

        public void Delete(string Id)
        {
            var productToRemove = products.Find(p => p.Id == Id);
            if (productToRemove != null)
                products.Remove(productToRemove);
            else
                throw new Exception("Product not found");
        }
    }
}
