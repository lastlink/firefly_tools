using System;
using System.Collections.Generic;
using System.IO;
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

                // var column1 = new List<string>();
                // var column2 = new List<string>();
                using (var rd = new StreamReader(inputfile.Value()))
                {
                    using (var csv_file = new StreamWriter(outputfile.Value()))
                    {
                        while (!rd.EndOfStream)
                        {

                            var splits = rd.ReadLine().Split(',');
                            Console.WriteLine(string.Join(",", splits));
                            // column1.Add(splits[0]);

                            // csv_file.WriteLine(string.Join(",", splits));
                            // column2.Add(splits[1]);
                        }
                    }
                }
                // Console.WriteLine("Column 1:");
                // foreach (var element in column1)
                //     Console.WriteLine(element);
                    
            });

            app.Execute(args);
        }
    }
}
