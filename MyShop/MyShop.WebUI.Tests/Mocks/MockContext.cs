﻿using MyShop.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShop.Core.Models;

namespace MyShop.WebUI.Tests.Mocks
{
    public class MockContext<T> : IRepository<T> where T : BaseEntity
    {
        private List<T> items;
        private string className;

        public MockContext()
        {

            items = new List<T>();
        }

        public void Commit()
        {

        }

        public void Insert(T t)
        {
            items.Add(t);
        }

        public void Update(T t)
        {
            T tToUpdate = items.Find(i => i.Id == t.Id);
            if (tToUpdate != null)
                tToUpdate = t;
            else
                throw new Exception($"{className} not found.");
        }

        public T Find(string id)
        {
            T t = items.Find(i => i.Id == id);
            if (t != null)
                return t;
            throw new Exception($"{className} not found.");
        }

        public IQueryable<T> Collection() => items.AsQueryable();

        public void Delete(string id)
        {
            T tToDelete = items.Find(i => i.Id == id);
            if (tToDelete != null)
                items.Remove(tToDelete);
            else
                throw new Exception($"{className} not found.");
        }
}

    
    
}
