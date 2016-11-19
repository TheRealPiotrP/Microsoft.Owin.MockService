using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Microsoft.Owin.MockService.Extensions.ODataV4
{
    public static class MockServiceExtensions
    {
        public static ResponseBuilder OnPostEntityRequest(this MockService mockService, string entitySetPath)
        {
            return mockService
                .OnRequest(c => c.Method == "POST" && c.Path.Value == entitySetPath);
        }

        public static ResponseBuilder OnPatchEntityRequest(this MockService mockService, string entityPath)
        {
            return mockService
                .OnRequest(c => c.Method == "PATCH" && c.Path.Value == entityPath);
        }

        public static ResponseBuilder OnGetEntityRequest(this MockService mockService, string entitySetPath)
        {
            return mockService
                .OnRequest(c => c.Method == "GET" && c.Path.Value == entitySetPath);
        }

        public static ResponseBuilder OnGetEntityWithExpandRequest(this MockService mockService, string entitySetPath,
            IEnumerable<string> expandTargets)
        {
            return mockService
                .OnRequest(c =>
                    c.Method == "GET" &&
                    c.Path.Value == entitySetPath &&
                    (c.Query["$expand"] == string.Join(",", expandTargets)));
        }

        public static ResponseBuilder OnGetEntityCountRequest(this MockService mockService, string entitySetPath)
        {
            return mockService
                .OnRequest(c => c.Method == "GET" && c.Path.Value == entitySetPath + "/$count");
        }

        public static ResponseBuilder OnGetEntityPropertyRequest(this MockService mockService, string entityPath,
            string propertyName)
        {
            return mockService
                .OnRequest(c => c.Method == "GET" && c.Path.Value == entityPath + "/" + propertyName);
        }

        public static ResponseBuilder OnPostEntityPropertyRequest(this MockService mockService, string entityPath,
            string propertyName)
        {
            return mockService
                .OnRequest(c => c.Method == "POST" && c.Path.Value == entityPath + "/" + propertyName);
        }

        public static ResponseBuilder OnInvokeMethodRequest(this MockService mockService, string httpMethod,
            string methodPath,
            TestReadableStringCollection uriArguments, JObject expectedBody)
        {
            uriArguments = uriArguments ?? new TestReadableStringCollection(new Dictionary<string, string[]>());

            return mockService
                .OnRequest(c =>
                    c.Method == httpMethod &&
                    c.Path.Value.StartsWith(methodPath) &&
                    c.InvokesMethodWithParameters(methodPath, uriArguments) &&
                    ((c.Body.Length == 0 && expectedBody == null) ||
                     (JToken.DeepEquals(expectedBody, c.Body.ToJObject()))));
        }
    }
}
