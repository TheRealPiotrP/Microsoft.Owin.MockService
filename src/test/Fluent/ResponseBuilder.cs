using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Kestrel.MockServer
{
    public class ResponseBuilder
    {
        private readonly MockService _mockService;

        private readonly Expression<Func<HttpRequest, bool>> _requestValidator;

        internal ResponseBuilder(MockService mockService, Expression<Func<HttpRequest, bool>> requestValidator)
        {
            _requestValidator = requestValidator;

            _mockService = mockService;
        }

        public MockService RespondWith(Action<HttpResponse> responseConfiguration)
        {
            if (responseConfiguration == null) throw new ArgumentNullException("responseConfiguration");

            Func<HttpResponse, Task<HttpResponse>> responseFunction = c =>
            {
                responseConfiguration(c);

                return null;
            };

            _mockService.Setup(_requestValidator, responseFunction);

            return _mockService;
        }

        public MockService RespondWith(Action<HttpResponse, string> responseConfiguration)
        {
            if (responseConfiguration == null) throw new ArgumentNullException("responseConfiguration");

            Func<HttpResponse, Task<HttpResponse>> responseFunction = c =>
            {
                responseConfiguration(c, _mockService.BaseAddress);

                return null;
            };

            _mockService.Setup(_requestValidator, responseFunction);

            return _mockService;
        }
    }
}