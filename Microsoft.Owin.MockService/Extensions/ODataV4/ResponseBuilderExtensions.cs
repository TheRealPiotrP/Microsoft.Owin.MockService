using Newtonsoft.Json.Linq;

namespace Microsoft.Owin.MockService.Extensions.ODataV4
{
    public static class ResponseBuilderExtensions
    {
        public static MockService RespondWithCreateEntity(this ResponseBuilder responseBuilder, string entitySetName, JObject response = null)
        {

            return responseBuilder.RespondWith((c, b) =>
            {
                c.StatusCode = 201;
                c.WithDefaultODataHeaders();
                c.WithODataEntityResponseBody(b, entitySetName, response);
            });
        }

        public static MockService RespondWithGetEntity(this ResponseBuilder responseBuilder, string entitySetName, JObject response = null)
        {

            return responseBuilder.RespondWith((c, b) =>
            {
                c.StatusCode = 200;
                c.WithDefaultODataHeaders();
                c.WithODataEntityResponseBody(b, entitySetName, response);
            });
        }

        public static MockService RespondWithODataOk(this ResponseBuilder responseBuilder)
        {
            return responseBuilder
                .RespondWith((c, b) =>
                {
                    c.StatusCode = 200;
                    c.WithDefaultODataHeaders();
                });
        }

        public static MockService RespondWithODataText(this ResponseBuilder responseBuilder, string text)
        {
            return responseBuilder
                .RespondWith((c, b) =>
                {
                    c.StatusCode = 200;
                    c.WithDefaultODataHeaders();
                    c.ContentType = "text/plain";
                    c.Write(text);
                });
        }
    }
}