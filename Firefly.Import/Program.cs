using System;
using System.Collections.Generic;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace Firefly.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()) // requires Microsoft.Extensions.Configuration.Json
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // requires Microsoft.Extensions.Configuration.Json
                    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                    .AddEnvironmentVariables(); 
            IConfigurationRoot configuration = builder.Build();
            Console.WriteLine(configuration.GetConnectionString("con"));
            Console.WriteLine(configuration.GetSection("Budget").GetSection("categories"));


            var token = configuration.GetSection("Budget:categories");
            Console.WriteLine("This is a token with key (" + token.Key + ") " + token.Value);

            Console.WriteLine(configuration.GetSection("Budget:categories").Value);
            
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
