using System;
using McMaster.Extensions.CommandLineUtils;

namespace Firefly.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "ConsoleArgs";
            app.Description = ".NET Core console app with argument parsing.";

            app.HelpOption();

            var inputfile = app.Option("-i|--ifile <inputfile>", "Required. The message.", CommandOptionType.SingleValue)
            .IsRequired();

            var outputfile = app.Option("-o|--ofile <outputfile>", "Required. The message.", CommandOptionType.SingleValue)
            .IsRequired();

            var bank = app.Option("-b|--bank <inputfile>", "Required. The message.", CommandOptionType.SingleValue)
            .IsRequired();



            // app.HelpOption("-?|-h|--help");
            app.OnExecute(() =>
            {
                // Console.WriteLine($"Input {inputfile}!");
                Console.WriteLine($"Input file is \" {inputfile.Value()}!");
                Console.WriteLine($"Output file is \" {outputfile.Value()}!");
                Console.WriteLine($"Bank file is \" {bank.Value()}!");


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
            });

            app.Execute(args);
        }
    }
}
