using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.Kestrel.MockServer
{
    public class Startup
    {
        private MockService _service;

        public Startup(IHostingEnvironment env)
        {
            _service = MockServiceRepository.GetServiceMockById(env.ApplicationName);            
        }


        public IConfigurationRoot Configuration { get; private set; }

        #region snippet_Configure
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            _service.Logger = loggerFactory.CreateLogger("MockLogger");

            app.UseMiddleware<RequestLoggingMiddleware>()
                .Run(async (context) =>
            {
                await _service.Invoke(context);
            });
        }
        #endregion
    }
}
