using SQLTranslator.Destination;
using System;
using System.Collections.Generic;
using System.IO;

namespace SQLTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                Console.WriteLine("Usage: SQLTranslator [input filename] [output filename]");
                Environment.Exit(0);
            }
            string fileName = args[0];
            string outputName = args[1];
            if (File.Exists(fileName))
            {
                AbstractDestinationFactory destinationFactory = new DestinationFactory();
                IDestination destination = destinationFactory.CreatePostgreSQLDestination();
                using (StreamReader inputStream = new StreamReader(fileName, encoding: System.Text.Encoding.GetEncoding("iso-8859-1")))
                {
                    using (StreamWriter outputStream = new StreamWriter(File.Open(outputName, FileMode.Create), encoding: System.Text.Encoding.UTF8))
                    {
                        Console.WriteLine("Beginning translation.");
                        string line;
                        string translatedLine;
                        do
                        {
                            line = inputStream.ReadLine();
                            line = line.Replace('`', '"');

                            //Case CREATE TABLE
                            if (line.StartsWith("CREATE TABLE"))
                            {
                                List<string> bloc = new List<string> { line };
                                do
                                {
                                    line = inputStream.ReadLine();
                                    line = line.Replace('`', '"');
                                    bloc.Add(line);
                                } while (!line.StartsWith(')'));
                                bloc = destination.GenerateTable(bloc);
                                foreach (string blocLine in bloc)
                                {
                                    outputStream.WriteLine(blocLine);
                                }
                            }

                            //Case INSERT INTO
                            else if (line.StartsWith("INSERT INTO"))
                            {
                                translatedLine = destination.GenerateInsert(line);
                                outputStream.WriteLine(translatedLine);
                            }

                            //Case DROP TABLE
                            else if (line.StartsWith("DROP TABLE"))
                            {
                                translatedLine = destination.GenerateDrop(line);
                                outputStream.WriteLine(translatedLine);
                            }


                        } while (!inputStream.EndOfStream);
                    }
                }
                Console.WriteLine("File written!");
            }
            else
            {
                Console.WriteLine("No file found.");
            }
        }
    }
}
