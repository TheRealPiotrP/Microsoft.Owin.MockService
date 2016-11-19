using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Its.Log.Instrumentation;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Linq;

namespace Microsoft.Owin.MockService
{
    public class MockService : IDisposable
    {
        private readonly int _portNumber;
        private readonly IDisposable _host;
        private readonly List<Tuple<Expression<Func<IOwinRequest, bool>>, Func<IOwinResponse, Task>>> _handlers;
        private readonly IList<Expression<Func<IOwinRequest, bool>>> _unusedHandlers;
        private readonly bool _ignoreUnusedHandlers;

        public MockService(bool ignoreUnusedHandlers = false)
        {
            _handlers = new List<Tuple<Expression<Func<IOwinRequest, bool>>, Func<IOwinResponse, Task>>>();
            _unusedHandlers = new List<Expression<Func<IOwinRequest, bool>>>();
            _ignoreUnusedHandlers = ignoreUnusedHandlers;

            MockServiceRepository.Register(_portNumber, this);

            _host = WebApp.Start<MockStartup>(GetBaseAddress());
        }

        internal MockService Setup(Expression<Func<IOwinRequest, bool>> condition, Func<IOwinResponse, Task> response)
        {
            _handlers.Add(new Tuple<Expression<Func<IOwinRequest, bool>>, Func<IOwinResponse, Task>>(condition, response));
            _unusedHandlers.Add(condition);

            Log.Write(new ConstantMemberEvaluationVisitor().Visit(condition));

            return this;
        }

        public ResponseBuilder OnRequest(Expression<Func<IOwinRequest, bool>> condition)
        {
            return new ResponseBuilder(this, condition);
        }

        public Task Invoke(IOwinContext context)
        {
            try
            {
                foreach (var handler in _handlers)
                {
                    if (!handler.Item1.Compile().Invoke(context.Request)) continue;

                    _unusedHandlers.Remove(handler.Item1);
                    return handler.Item2(context.Response);
                }

                context.Response.StatusCode = 404;
                Debug.WriteLine("No handler for request\n\r{0} {1}", context.Request.Method, context.Request.Path);
                return Task.FromResult<object>(null);
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                context.Response.Write(JObject.FromObject(e).ToString());
                return Task.FromResult<object>(null);
            }
        }

        public string GetBaseAddress()
        {
            return String.Format("http://localhost:{0}/", _portNumber);
        }

        public void Dispose()
        {
            _host.Dispose();

            MockServiceRepository.Unregister(_portNumber);

            if (!_ignoreUnusedHandlers && _unusedHandlers.Any())
                throw new InvalidOperationException(
                    String.Format("Mock Server {0} expected requests \r\n\r\n {1} \r\n\r\n but they were not made.",
                        GetBaseAddress(),
                        _unusedHandlers.Select(h => new ConstantMemberEvaluationVisitor().Visit(h).ToString())
                            .Aggregate((c, n) => c + "\r\n" + n)));
        }
    }
}