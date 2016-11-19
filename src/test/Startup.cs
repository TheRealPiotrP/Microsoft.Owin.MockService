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

            app.Run(async (context) =>
            {
                var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();

                context.Response.ContentType = "text/html";

                await context.Response
                    .WriteAsync("<p>Hosted by Kestrel</p>");

                if (serverAddressesFeature != null)
                {
                    await context.Response
                        .WriteAsync("<p>Listening on the following addresses: " +
                            string.Join(", ", serverAddressesFeature.Addresses) +
                            "</p>");
                }

                await context.Response.WriteAsync($"<p>Request URL: {context.Request.GetDisplayUrl()}<p>");
            });
        }
        #endregion
    }
}
