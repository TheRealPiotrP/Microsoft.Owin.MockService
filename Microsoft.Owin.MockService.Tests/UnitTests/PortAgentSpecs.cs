using System;
using System.Net;
using System.Net.Sockets;
using FluentAssertions;
using Microsoft.Owin.MockService.Agents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Owin.MockService.Tests
{
    [TestClass]
    public class PortAgentSpecs
    {
        [TestMethod]
        public void When_GetAvailablePort_is_called_Then_it_returns_an_open_port()
        {
            var freePortNumber = PortAgent.GetFreePortNumber();

            Action usePort = () =>
            {
                var tcpListener = new TcpListener(IPAddress.Loopback, freePortNumber);

                try
                {
                    tcpListener.Start();
                }
                finally
                {
                    tcpListener.Stop();
                }
            };

            usePort.
                ShouldNotThrow("Because PortAgent identified port {0} as available.", freePortNumber);
        }
    }
}
