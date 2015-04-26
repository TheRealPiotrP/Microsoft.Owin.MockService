using System;
using System.Net.Http;
using FluentAssertions;
using Microsoft.Its.Recipes;
using Microsoft.Owin.MockService.Repositories;
using Xunit;

namespace Microsoft.Owin.MockService.Tests.UnitTests
{
    public class MockServiceSpecs
    {
        [Fact]
        public void When_cretated_Then_it_registers_itself_with_the_MockServiceRepository()
        {
            using (var mockService = new MockService())
            {
                var portNumber = new Uri(mockService.GetBaseAddress()).Port;

                MockServiceRepository.GetServiceMockForPort(portNumber)
                    .Should().Be(mockService, "Because the MockService should be registered at its port number.");
            }
        }

        [Fact]
        public void When_disposed_Then_it_removes_itself_from_the_MockServiceRepository()
        {
            var portNumber = -1;

            using (var mockService = new MockService())
            {
                portNumber = new Uri(mockService.GetBaseAddress()).Port;
            }

            MockServiceRepository.GetServiceMockForPort(portNumber)
                    .Should().BeNull("Because the MockService should unregister itself during disposal.");
        }

        [Fact]
        public void When_request_condition_met_Then_provides_response()
        {
            var path = Any.Uri().AbsolutePath;

            var response = Any.Paragraph();

            var client = new HttpClient();

            using (var mockService = new MockService()
                .OnRequest(r => r.Path.ToString() == path)
                .RespondWith(r => r.Write(response)))
            {
                client.BaseAddress = new Uri(mockService.GetBaseAddress());

                client.GetStringAsync(path).Result
                    .Should().Be(response);
            }
        }

        [Fact]
        public void When_request_condition_met_Then_provides_response_with_baseAddress()
        {
            var path = Any.Uri().AbsolutePath;

            var response = Any.Paragraph();

            var client = new HttpClient();

            using (var mockService = new MockService()
                .OnRequest(r => r.Path.ToString() == path)
                .RespondWith((r,s) => r.Write(s)))
            {
                client.BaseAddress = new Uri(mockService.GetBaseAddress());

                client.GetStringAsync(path).Result
                    .Should().Be(mockService.GetBaseAddress());
            }
        }

        [Fact]
        public void When_request_not_expected_Then_responds_404()
        {
            var client = new HttpClient();

            HttpResponseMessage response;

            using (var mockService = new MockService())
            {
                client.BaseAddress = new Uri(mockService.GetBaseAddress());

                response = client.GetAsync("").Result;
            }

            response
                .StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound, "Because the request was not expected");
        }

        [Fact]
        public void When_request_processing_throws_an_exception_Then_responds_500_with_exception_body()
        {
            var client = new HttpClient();

            var exceptionMessage = Any.String();

            HttpResponseMessage response;

            using (var mockService = new MockService()
                .OnRequest(r => true)
                .RespondWith(r => { throw new Exception(exceptionMessage); }))
            {
                client.BaseAddress = new Uri(mockService.GetBaseAddress());

                response = client.GetAsync("").Result;
            }

            response.StatusCode
                .Should()
                .Be(System.Net.HttpStatusCode.InternalServerError, "Because request processing threw an exception");

            response.Content.ReadAsStringAsync().Result
                .Should().Contain(exceptionMessage);
        }

        [Fact]
        public void When_an_expected_request_is_not_made_Then_the_MockServer_throws_on_Dispose()
        {
            var mockService = new MockService()
                .OnRequest(r => true)
                .RespondWith(r => r.Write("No request for this response."));

            Action dispose = mockService.Dispose;

            dispose
                .ShouldThrow<InvalidOperationException>(
                    "Because Dispose should not hvae been called before all requests were made");
        }
    }
}
