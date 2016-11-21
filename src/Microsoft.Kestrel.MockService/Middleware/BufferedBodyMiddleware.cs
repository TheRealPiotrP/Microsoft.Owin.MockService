using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Microsoft.Owin.MockService.Middleware
{
    class BufferedBodyMiddleware 
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<BufferedBodyMiddleware> _logger;
    
        public BufferedBodyMiddleware(RequestDelegate next, ILogger<BufferedBodyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Body != null)
            {
                var requestBodyBuffer = new MemoryStream();
                await context.Request.Body.CopyToAsync(requestBodyBuffer);
                requestBodyBuffer.Seek(0, SeekOrigin.Begin);
                context.Request.Body = requestBodyBuffer;
            }

            var responseBodyStream = context.Response.Body;
            var responseBodyBuffer = new MemoryStream();
            context.Response.Body = responseBodyBuffer;

            await _next.Invoke(context);

            responseBodyBuffer.Seek(0, SeekOrigin.Begin);
            await responseBodyBuffer.CopyToAsync(responseBodyStream);
        }
    }
}