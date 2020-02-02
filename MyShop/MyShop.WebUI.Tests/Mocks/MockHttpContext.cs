using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.WebUI.Tests.Mocks
{
    public class MockHttpContext : HttpContextBase
    {
        private MockRequest MockRequest;
        private MockResponse MockResponse;
        private HttpCookieCollection Cookies;

        public MockHttpContext()
        {
            Cookies = new HttpCookieCollection();
            MockRequest = new MockRequest(Cookies);
            MockResponse = new MockResponse(Cookies);
        }

        public override HttpRequestBase Request => MockRequest;
        public override HttpResponseBase Response => MockResponse;

    }

    public class MockResponse : HttpResponseBase
    {
        private readonly HttpCookieCollection cookies;

        public MockResponse(HttpCookieCollection cookies)
        {
            this.cookies = cookies;
        }

        public override HttpCookieCollection Cookies => cookies;
    }

    public class MockRequest : HttpRequestBase
    {
        private readonly HttpCookieCollection cookies;

        public MockRequest(HttpCookieCollection cookies)
        {
            this.cookies = cookies;
        }

        public override HttpCookieCollection Cookies => cookies;
    }
}
