using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;

namespace MyShop.Services
{
    public class BasketService : IBasketService
    {
        private IRepository<Product> ProductContext;
        private IRepository<Basket> BasketContext;

        public const string BasketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> productContext, IRepository<Basket> basketContext)
        {
            ProductContext = productContext;
            BasketContext = basketContext;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
        {
            var cookie = httpContext.Request.Cookies.Get(BasketSessionName);
            var basket = new Basket();
            if (cookie != null)
            {
                string basketId = cookie.Value;
                if (!string.IsNullOrEmpty(basketId))
                {
                    basket = BasketContext.Find(basketId);
                }
                else
                {
                    if (createIfNull)
                    {
                        basket = CreateNewBasket(httpContext);
                    }
                }
            }
            else
            {
                if (createIfNull)
                {
                    basket = CreateNewBasket(httpContext);
                }
            }

            return basket;
        }

        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            var basket = new Basket();
            BasketContext.Insert(basket);
            BasketContext.Commit();

            var cookie = new HttpCookie(BasketSessionName)
            {
                Value = basket.Id,
                Expires = DateTime.Now.AddDays(1)
            };
            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext, string productId)
        {
            var basket = GetBasket(httpContext, true);
            var item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                item = new BasketItem
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quantity = 1
                };

                basket.BasketItems.Add(item);
            }
            else
            {
                item.Quantity += 1;
            }

            BasketContext.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if (item != null)
            {
                basket.BasketItems.Remove(item);
                BasketContext.Commit();
            }
        }

        public List<BasketItemViewModel> GetBasketItems(HttpContextBase httpContext)
        {
            var basket = GetBasket(httpContext, false);
            if (basket == null) return new List<BasketItemViewModel>();
            var results = (
                from b in basket.BasketItems
                join p in ProductContext.Collection() on b.ProductId equals p.Id
                select new BasketItemViewModel
                {
                    Id = b.Id,
                    Quantity = b.Quantity,
                    ProductName = p.Name,
                    Image = p.Image,
                    Price = p.Price
                }
            ).ToList();

            return results;

        }

        public BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            BasketSummaryViewModel viewModel = new BasketSummaryViewModel(0, 0);

            if (basket == null) return viewModel;
            
            int? basketCount = (
                from item in basket.BasketItems
                select item.Quantity).Sum();
            decimal? basketTotal = (
                from item in basket.BasketItems
                join p in ProductContext.Collection() on item.ProductId equals p.Id
                select item.Quantity * p.Price).Sum();
                
            viewModel.BasketCount = basketCount ?? 0;
            viewModel.BasketTotal = basketTotal ?? decimal.Zero;

            return viewModel;
        }

        public void ClearBasket(HttpContextBase httpContext)
        {
            var basket = GetBasket(httpContext, false);
            basket.BasketItems.Clear();
            BasketContext.Commit();
        }
    }
}
