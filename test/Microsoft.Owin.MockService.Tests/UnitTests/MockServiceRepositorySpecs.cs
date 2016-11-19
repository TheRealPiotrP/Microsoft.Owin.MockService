using System;
using System.Net;
using System.Net.Sockets;
using FluentAssertions;
using Microsoft.Its.Recipes;
using Microsoft.Owin.MockService.Agents;
using Microsoft.Owin.MockService.Repositories;
using Xunit;

namespace Microsoft.Owin.MockService.Tests.UnitTests
{
    public class MockServiceRepositorySpecs
    {
        [Fact]
        public void When_GetServiceMockForPort_called_with_unregistered_port_Then_returns_null()
        {
            MockServiceRepository.GetServiceMockForPort(Any.Int())
                .Should().BeNull("Because no MockService is registered at this port");
        }

        [Fact]
        public void When_GetServiceMockForPort_called_with_registered_port_Then_returns_registered_MockService()
        {
            var mockService = new MockService();

            var portNumber = Any.PositiveInt(65535);

            MockServiceRepository.Register(portNumber, mockService);

            MockServiceRepository.GetServiceMockForPort(portNumber)
                .Should().Be(mockService, "Because this is the MockService registered at this port");

            MockServiceRepository.Unregister(portNumber);
        }

        [Fact]
        public void When_Unregister_called_with_registered_port_Then_removes_MockService()
        {
            var mockService = new MockService();

            var portNumber = Any.PositiveInt(65535);

            MockServiceRepository.Register(portNumber, mockService);

            MockServiceRepository.Unregister(portNumber);

            MockServiceRepository.GetServiceMockForPort(portNumber)
                .Should().BeNull("Because the port was unregistered");
        }

        [Fact]
        public void When_Unregister_called_with_unregistered_port_Then_throws_with_useful_message()
        {
            var portNumber = Any.PositiveInt(65535);

            Action unregister = () => MockServiceRepository.Unregister(portNumber);

            unregister
                .ShouldThrow<InvalidOperationException>("Because that port was not registered")
                .WithMessage("port not registered", "Because that helps debug the issue");
        }

        [Fact]
        public void When_Register_called_with_registered_port_Then_throws_with_useful_message()
        {
            var mockService = new MockService();

            var portNumber = Any.PositiveInt(65535);

            MockServiceRepository.Register(portNumber, mockService);

            Action register = () => MockServiceRepository.Register(portNumber, mockService);

            register
                .ShouldThrow<InvalidOperationException>("Because that port is already registered")
                .WithMessage("port in use", "Because that helps debug the issue");
        }
    }
}
