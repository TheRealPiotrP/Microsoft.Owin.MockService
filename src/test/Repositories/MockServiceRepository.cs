// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Microsoft.Kestrel.MockServer
{
    internal static class MockServiceRepository
    {
        private static readonly IDictionary<string, MockService> MockServices =
            new Dictionary<string, MockService>();
        
        internal static MockService GetServiceMockById(string mockServiceId)
        {
            return MockServices.ContainsKey(mockServiceId) ? MockServices[mockServiceId] : null;
        }

        internal static void Register(MockService mockService)
        {
            lock (MockServices)
            {
                if (MockServices.ContainsKey(mockService.ServiceId))
                {
                    throw new InvalidOperationException("Id in use");
                }
                
                MockServices[mockService.ServiceId] = mockService;
            }
        }

        internal static void Unregister(string mockServiceId)
        {
            lock (MockServices)
            {
                if (!MockServices.ContainsKey(mockServiceId))
                    throw new InvalidOperationException("Id not registered");

                MockServices.Remove(mockServiceId);
            }
        }
    }
}