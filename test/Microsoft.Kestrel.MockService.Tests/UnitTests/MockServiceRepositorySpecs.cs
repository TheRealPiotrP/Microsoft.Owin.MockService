using System;
using FluentAssertions;
using Microsoft.Its.Recipes;
using Microsoft.Kestrel.MockServer;
using Xunit;

namespace Microsoft.Kestrel.MockServer.Tests.UnitTests
{
    public class MockServiceRepositorySpecs
    {
        [Fact]
        public void When_GetServiceMockById_called_with_unregistered_Id_Then_returns_null()
        {
            MockServiceRepository.GetServiceMockById(Any.String())
                .Should().BeNull("Because no MockService is registered at this port");
        }

        [Fact]
        public void When_GetServiceMockById_called_with_registered_port_Then_returns_registered_MockService()
        {
            var mockService = new MockService();

            MockServiceRepository.GetServiceMockById(mockService.ServiceId)
                .Should().Be(mockService, "Because the MockService self-registered.");

            MockServiceRepository.Unregister(mockService);
        }

        [Fact]
        public void When_Unregister_called_with_registered_MockService_Then_removes_MockService()
        {
            var mockService = new MockService();

            MockServiceRepository.Unregister(mockService);

            MockServiceRepository.GetServiceMockById(mockService.ServiceId)
                .Should().BeNull("Because the mockService was unregistered");
        }

        [Fact]
        public void When_Unregister_called_with_unregistered_MockService_Then_throws_with_useful_message()
        {
            var mockService = new MockService();

            MockServiceRepository.Unregister(mockService);

            Action unregister = () => MockServiceRepository.Unregister(mockService);

            unregister
                .ShouldThrow<InvalidOperationException>("Because that MockService was not registered")
                .WithMessage("MokService not registered", "Because that helps debug the issue");
        }

        [Fact]
        public void When_Register_called_with_registered_MockService_Then_throws_with_useful_message()
        {
            var mockService = new MockService();

            Action register = () => MockServiceRepository.Register(mockService);

            register
                .ShouldThrow<InvalidOperationException>("Because that MockService is already registered")
                .WithMessage("ServiceId in use", "Because that helps debug the issue");
        }
    }
}
