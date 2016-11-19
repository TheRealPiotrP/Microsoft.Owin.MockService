using System;

namespace Microsoft.Kestrel.MockServer
{
    class Program
    {
        public static int Main(string[] args)
        {
            using (var ms = new MockService())
            {
                Console.WriteLine(ms.BaseAddress);

                using (var ms2 = new MockService())
                {
                    Console.WriteLine(ms2.BaseAddress);

                    Console.ReadLine();
                }
            }
            using (var ms = new MockService())
            {
                Console.WriteLine(ms.BaseAddress);

                Console.ReadLine();
            }

            return 0;
        }
    }
}