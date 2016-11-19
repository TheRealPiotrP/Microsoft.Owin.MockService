// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Owin.MockService.Repositories
{
    internal static class MockServiceRepository
    {
        private static readonly IDictionary<int, MockService> MockServices =
            new Dictionary<int, MockService>();
        
        internal static MockService GetServiceMockForPort(int portNumber)
        {
            return MockServices.ContainsKey(portNumber) ? MockServices[portNumber] : null;
        }

        internal static void Register(int portNumber, MockService mockService)
        {
            lock (MockServices)
            {
                if (MockServices.ContainsKey(portNumber))
                    throw new InvalidOperationException("port in use");

                MockServices[portNumber] = mockService;
            }
        }

        internal static void Unregister(int portNumber)
        {
            lock (MockServices)
            {
                if (!MockServices.ContainsKey(portNumber))
                    throw new InvalidOperationException("port not registered");

                MockServices.Remove(portNumber);
            }
        }
    }
}