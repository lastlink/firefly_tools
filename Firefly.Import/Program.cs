using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using LumenWorks.Framework.IO.Csv;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace Firefly.Import
{
    class Program
    {
        static IConfigurationRoot configuration = null;
        static void Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()) // requires Microsoft.Extensions.Configuration.Json
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // requires Microsoft.Extensions.Configuration.Json
                    .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                    .AddEnvironmentVariables();
            configuration = builder.Build();
            Console.WriteLine(configuration.GetConnectionString("con"));
            Console.WriteLine(configuration.GetSection("Budget").GetSection("categories"));



            var token = configuration.GetSection("Budget:categories");
            Console.WriteLine("This is a token with key (" + token.Key + ") " + token.Value);
            Console.WriteLine(configuration.GetSection("Budget:categories").Value);
            Console.WriteLine(configuration.GetSection("Account:accounts").Value);
            // string withspace="AMZ*Buyquest, Inc. amzn.com/pmts NY          06/11";
            // Console.WriteLine(withspace);
            // withspace=Regex.Replace(withspace, " {2,}", " ");
            // Console.WriteLine(withspace);

            // return;

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

            String[] outputFormat = new string[] {"Notes", "Posting Date", "Description",
                            "Amount (Debit)", "Amount (Credit)", "Budget", "Balance", "Account"};



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
                using (CsvReader lineArr = new CsvReader(rd.BaseStream, false, rd.CurrentEncoding, true))
                {
                    lineArr.MissingFieldAction = MissingFieldAction.ReplaceByNull;
                    int fieldCount = lineArr.FieldCount;
                    string[] baseHeader = new string[lineArr.FieldCount];
                    using (var csv_file = new StreamWriter(outputfile.Value()))
                    {
                        csv_file.WriteLine(string.Join(",", outputFormat));
                        int lineNum = 0;
                        while (lineArr.ReadNextRecord())
                        {
                            // TODO:  change delimiter to ;
                            // string[] lineArr =  rd.ReadLine().Split(",");
                            // Console.WriteLine(string.Join(",",outputFormat));
                            // return;
                            // withspace=Regex.Replace(withspace, " {2,}", " ");

                            string[] rowResult = new string[outputFormat.Length];

                            switch (bank.Value())
                            {
                                case "chase":
                                    // has header
                                    if (lineNum == 0)
                                    {
                                        // baseHeader = lineArr.GetFieldHeaders();
                                        for (int i = 0; i < baseHeader.Length; i++)
                                        {
                                            baseHeader[i] = lineArr[i];
                                        }
                                        // baseHeader = lineArr;
                                        Console.WriteLine(string.Join(",", baseHeader));
                                        rowResult = null;
                                    }
                                    else
                                    {
                                        if (lineArr[Array.FindIndex(baseHeader, h => h == "Details")] == "DEBIT")
                                        {
                                            rowResult[Array.FindIndex(outputFormat, h => h == "Amount (Debit)")] = lineArr[Array.FindIndex(baseHeader, h => h == "Amount")];
                                        }
                                        else if (lineArr[Array.FindIndex(baseHeader, h => h == "Details")] == "CREDIT")
                                        {
                                            rowResult[Array.FindIndex(outputFormat, h => h == "Amount (Credit)")] = lineArr[Array.FindIndex(baseHeader, h => h == "Amount")];

                                        }
                                        else
                                            Console.WriteLine(
                                                "Missing credit/debit:" + lineArr[Array.FindIndex(baseHeader, h => h == "Details")] + " on line:" + lineNum);



                                        rowResult[Array.FindIndex(outputFormat, h => h == "Notes")] = lineArr[Array.FindIndex(baseHeader, h => h == "Details")];
                                        // RegexOptions options = RegexOptions.None;
                                        // Regex regex = new Regex("[ ]{2,}", options);  

                                        string desc = lineArr[Array.FindIndex(baseHeader, h => h == "Description")].ToString();
                                        desc = Regex.Replace(desc, " {2,}", " ");
                                        Console.WriteLine("desc:" + desc);
                                        //  Regex.Replace(desc, " {2,}", " ");
                                        rowResult[Array.FindIndex(outputFormat, h => h == "Description")] = desc;
                                        // Regex.Replace(myString, " {2,}", " ");
                                        //  regex.Replace(lineArr[Array.FindIndex(baseHeader, h => h == "Description")].ToString()," ").Replace(System.Environment.NewLine, "");

                                        // regex.Replace(lineArr[Array.FindIndex(baseHeader, h => h == "Description")], "\\s\\s+", " ").Replace(System.Environment.NewLine, "replacement text");
                                        // # need to clean out junk
                                        rowResult[Array.FindIndex(outputFormat, h => h ==
                                            "Account")] = cleanAccount(rowResult[Array.FindIndex(outputFormat, h => h == "Description")]);
                                        rowResult[Array.FindIndex(outputFormat, h => h ==
                                            "Posting Date")] = lineArr[Array.FindIndex(baseHeader, h => h == "Posting Date")];
                                        rowResult[Array.FindIndex(outputFormat, h => h ==
                                            "Balance")] = lineArr[Array.FindIndex(baseHeader, h => h == "Balance")];
                                    }
                                    break;
                                case "wells":
                                    if (lineNum == 0)
                                    {
                                        baseHeader = new string[] { "Posting Date", "Amount", "Star", "Blank", "Description" };
                                        Console.WriteLine(string.Join(",", baseHeader));
                                    }

                                    rowResult[Array.FindIndex(outputFormat, h => h == "Description")] = Regex.Replace(lineArr[Array.FindIndex(baseHeader, h => h == "Description")],
                            "\\s\\s+", " ");

                                    if (decimal.Parse(lineArr[Array.FindIndex(baseHeader, h => h == "Amount")]) > 0)
                                        rowResult[Array.FindIndex(outputFormat, h => h ==
                                                "Amount (Credit)")] = lineArr[Array.FindIndex(baseHeader, h => h == "Amount")];
                                    else
                                    {
                                        // decimal amount;
                                        // bool result = decimal.TryParse(lineArr[Array.FindIndex(baseHeader, h => h == "Amount")], out amount);
                                        // if (result)
                                        // {
                                        //     rowResult[Array.FindIndex(outputFormat, h => h ==
                                        //        "Amount (Debit)")] = Math.Abs(amount).ToString();
                                        // }
                                        // should always have an amount or invalid
                                        rowResult[Array.FindIndex(outputFormat, h => h ==
                                                "Amount (Debit)")] = Math.Abs(decimal.Parse(lineArr[Array.FindIndex(baseHeader, h => h == "Amount")])).ToString();

                                    }
                                    rowResult[Array.FindIndex(outputFormat, h => h ==
                                            "Posting Date")] = lineArr[Array.FindIndex(baseHeader, h => h == "Posting Date")];

                                    rowResult[Array.FindIndex(outputFormat, h => h ==
                                            "Account")] = cleanAccount(rowResult[Array.FindIndex(outputFormat, h => h == "Description")]);
                                    break;

                                default:
                                    Console.WriteLine("bank not implemented");
                                    return;
                            }
                            if (rowResult != null)
                            {
                                rowResult[Array.FindIndex(outputFormat, h => h ==
                              "Budget")] = determineBudget(rowResult[Array.FindIndex(outputFormat, h => h == "Account")]);


                                if (rowResult[Array.FindIndex(outputFormat, h => h == "Description")].Contains(","))
                                    rowResult[Array.FindIndex(outputFormat, h => h == "Description")] = "\"" + rowResult[Array.FindIndex(outputFormat, h => h == "Description")] + "\"";
                                // Console.WriteLine(string.Join(",", rowResult));
                                // column1.Add(splits[0]);

                                csv_file.WriteLine(string.Join(",", rowResult));
                            }
                            // csv_file.WriteLine(string.Join(",", splits));
                            // column2.Add(splits[1]);
                            lineNum++;
                        }
                    }
                }
                // Console.WriteLine("Column 1:");
                // foreach (var element in column1)
                //     Console.WriteLine(element);

            });

            app.Execute(args);
        }

        static string cleanAccount(string account)
        {
            var tmpAccount = account;
            // try to remove m/d/y first
            tmpAccount = Regex.Replace(tmpAccount, "((0|1)\\d{1})\\/((0|1|2)\\d{1})\\/(\\d{2})", " ");
            // # remove m/d
            tmpAccount = Regex.Replace(tmpAccount, "((0|1)\\d{1})\\/((0|1|2)\\d{1})", " ");

            // # remove double spaces
            tmpAccount = Regex.Replace(tmpAccount, "\\s\\s+", " ");

            foreach (var accountType in configuration.GetSection("Account:accounts").Value.Split(","))
            {
                string[] accountSearch = Array.ConvertAll(configuration.GetSection("Account:" + accountType + "_search").Value.ToString().Split(","), d => d.ToUpper());
                foreach (string word in accountSearch)
                {
                    if (account.Contains(word.ToUpper()))
                    {
                        return string.IsNullOrEmpty(configuration.GetSection("Account:" + accountType + "_name").Value) ? accountType : configuration.GetSection("Account:" + accountType + "_name").Value;
                    }
                }
            }
            // # could possibly do matching from setting ini as well, but would be better to clean unique ideas and do a map
            if (tmpAccount.Contains(","))
                return "\"" + tmpAccount + "\"";
            return tmpAccount;
        }

        static string determineBudget(string account)
        {
            foreach (var category in configuration.GetSection("Budget:categories").Value.Split(","))
            {
                // Console.WriteLine(category);
                // Console.WriteLine(string.IsNullOrEmpty(configuration.GetSection("Budget:categories").Value));
                if (!string.IsNullOrEmpty(configuration.GetSection("Budget:" + category + "_search").Value))
                {
                    string[] categorySearch = Array.ConvertAll(configuration.GetSection("Budget:" + category + "_search").Value.ToString().Split(","), d => d.ToUpper());

                    foreach (string word in categorySearch)
                    {
                        if (account.Contains(word.ToUpper()))
                        {
                            // any custom name mappings from settings must have escapes for \"
                            // e.g. "Tranportation_name": "\"TRANSPORTATION:Auto Gas, public trans, parking\"",
                            return string.IsNullOrEmpty(configuration.GetSection("Budget:" + category + "_name").Value) ? category : configuration.GetSection("Budget:" + category + "_name").Value;
                        }
                    }

                }

            }
            return "";
        }
    }
}
