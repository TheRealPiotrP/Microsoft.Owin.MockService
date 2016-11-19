// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;

namespace Microsoft.Owin.MockService.Agents
{
    public static class PortAgent
    {
        public static int GetFreePortNumber()
        {
            var freePort = 0;

            var tcpListener = new TcpListener(IPAddress.Loopback, 0);

            try
            {
                tcpListener.Start();
                freePort = ((IPEndPoint) tcpListener.LocalEndpoint).Port;
            }
            finally
            {
                tcpListener.Stop();
            }
            
            return freePort;
        }
    }
}