using System;
using System.Threading;


namespace test_app
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("START of test_app!");
            // Loop
            for (int i=0; i<10; i++)
            {
                Thread.Sleep(500);
                Console.Write(".");
            }
            Console.WriteLine("\nFINISH!");
        }
    }
}
