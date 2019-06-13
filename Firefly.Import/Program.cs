using System;

namespace Firefly.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine($"Hello {args[0]}!");
                Console.WriteLine($"Hello {args[1]}!");
            }
            else
            {
                Console.WriteLine("Hello!");
            }
            Console.WriteLine(string.Join(",", args));
            Console.WriteLine("Hello World!");
        }
    }
}
