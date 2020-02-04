using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.Services;
using MyShop.WebUI.Controllers;
using MyShop.WebUI.Tests.Mocks;

namespace MyShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class BasketControllerTests
    {
        [TestMethod]
        public void CanAddBasketItem()
        {
            // setup
            var baskets = new MockContext<Basket>();
            var products = new MockContext<Product>();
            var orders = new MockContext<Order>();
            var httpContext = new MockHttpContext();
            var customers = new MockContext<Customer>();

            var basketService = new BasketService(products, baskets);
            var orderService = new OrderService(orders);

            customers.Insert(new Customer {Id = "1", Email = "luka.kljajevic@email.com", ZipCode = "11000"});
            var fakeUser = new GenericPrincipal(new GenericIdentity("luka.kljajevic@email.com", "Forms"), null);
            httpContext.User = fakeUser;
            var controller = new BasketController(basketService, orderService, customers);

            controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);

            // act
            //basketService.AddToBasket(httpContext, "1");
            controller.AddToBasket("1");

            var basket = baskets.Collection().FirstOrDefault();

            // assert
            Assert.IsNotNull(basket);
            Assert.AreEqual(1, basket.BasketItems.Count);
            Assert.AreEqual("1", basket.BasketItems.ToList().FirstOrDefault().ProductId);
        }

        [TestMethod]
        public void CanGetSummaryViewModel()
        {
            var baskets = new MockContext<Basket>();
            var products = new MockContext<Product>();
            var orders = new MockContext<Order>();
            var httpContext = new MockHttpContext();
            var customers = new MockContext<Customer>();

            products.Insert(new Product() { Id = "1", Price = 10.00m });
            products.Insert(new Product() { Id = "2", Price = 5.00m });

            var basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 1 });

            baskets.Insert(basket);

            var basketService = new BasketService(products, baskets);
            var orderService = new OrderService(orders);
            

            customers.Insert(new Customer { Id = "1", Email = "luka.kljajevic@email.com", ZipCode = "11000" });
            var fakeUser = new GenericPrincipal(new GenericIdentity("luka.kljajevic@email.com", "Forms"), null);
            httpContext.User = fakeUser;
            var controller = new BasketController(basketService, orderService, customers);
            httpContext.Request.Cookies.Add(new HttpCookie("eCommerceBasket") { Value = basket.Id });
            controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);

            var result = controller.BasketSummary() as PartialViewResult;
            var basketSummary = (BasketSummaryViewModel) result.ViewData.Model;

            Assert.AreEqual(3, basketSummary.BasketCount);
            Assert.AreEqual(25.00m, basketSummary.BasketTotal);
        }

        [TestMethod]
        public void CanCheckoutAndCreateOrder()
        {
            var products = new MockContext<Product>();
            products.Insert(new Product() { Id = "1", Price = 10.00m });
            products.Insert(new Product() { Id = "2", Price = 5.00m });

            var baskets = new MockContext<Basket>();
            var basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 1 });
            baskets.Insert(basket);

            var basketService = new BasketService(products, baskets);
            var orders = new MockContext<Order>();
            var orderService = new OrderService(orders);
            var customers = new MockContext<Customer>();
            var controller = new BasketController(basketService, orderService, customers);
            var httpContext = new MockHttpContext();
            httpContext.Request.Cookies.Add(new HttpCookie("eCommerceBasket") { Value = basket.Id });
            controller.ControllerContext = new ControllerContext(httpContext, new RouteData(), controller);

            var order = new Order();
            var result = controller.Checkout(order);

            Assert.AreEqual(2, order.OrderItems.Count);
            Assert.AreEqual(0, basket.BasketItems.Count);

            var orderInRepository = orders.Find(order.Id);
            Assert.AreEqual(2, orderInRepository.OrderItems.Count);
        }
    }
}
