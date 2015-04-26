﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Its.Log.Instrumentation;

namespace Microsoft.Owin.MockService.Middleware
{
    class LoggingMiddleware : OwinMiddleware
    {
        public LoggingMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            await WriteRequestSummary(context);

            var bodyStream = context.Response.Body;
            var bodyBuffer = new MemoryStream();
            context.Response.Body = bodyBuffer;

            await Next.Invoke(context);

            var responseBody = await ReadResponseBody(bodyBuffer);

            WriteResponseSummary(context, responseBody);

            bodyBuffer.Seek(0, SeekOrigin.Begin);
            await bodyBuffer.CopyToAsync(bodyStream);
        }

        private static async Task<string> ReadResponseBody(MemoryStream bodyBuffer)
        {
            bodyBuffer.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(bodyBuffer);
            var responseBody = await reader.ReadToEndAsync();
            return responseBody;
        }

        private static void WriteResponseSummary(IOwinContext context, string responseBody)
        {
            Log.Write(String.Format(@"
<<<RESPONSE<<<<<<<<<<<<<<<<<<<<<<<<<<<<
HTTP/{1} {0}
{2}
{3}
<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<",
                (int) context.Response.StatusCode, context.Response.ReasonPhrase,
                String.Join("",
                    context.Response.Headers.Select(
                        h =>
                            String.Format("{0}: {1}\r\n", h.Key,
                                String.Join(", ", h.Value.Select(v => v.ToString()))))), responseBody));
        }

        private static async Task WriteRequestSummary(IOwinContext context)
        {
            var requestBody = "";

            if (context.Request.Body != null)
            {
                var bodyBuffer = new MemoryStream();
                await context.Request.Body.CopyToAsync(bodyBuffer);
                bodyBuffer.Seek(0, SeekOrigin.Begin);
                context.Request.Body = bodyBuffer;
                requestBody = await new StreamReader(bodyBuffer).ReadToEndAsync();
                bodyBuffer.Seek(0, SeekOrigin.Begin);
            }

            Log.Write(String.Format(@"
>>>REQUEST>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
{0} {1}
{2}
{3}
>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>",
                context.Request.Method, context.Request.Uri,
                String.Join("",
                    context.Request.Headers.Select(
                        h =>
                            String.Format("{0}: {1}\r\n", h.Key,
                            String.Join(", ", h.Value.Select(v => v.ToString(CultureInfo.InvariantCulture)))))),
                            requestBody));
        }
    }
}
