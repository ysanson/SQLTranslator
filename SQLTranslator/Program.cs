using SQLTranslator.Destination;
using System;
using System.Collections.Generic;
using System.IO;

namespace SQLTranslator
{
    /// <summary>
    /// Main program of this application. It takes two arguments, the input file and the output file.
    /// The output is (re)created.
    /// </summary>
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
                ITranslator translator = destinationFactory.CreateSQLTranslator(AbstractDestinationFactory.Destinations.PostgreSQL11);
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
                                bloc = translator.GenerateTable(bloc);
                                foreach (string blocLine in bloc)
                                {
                                    outputStream.WriteLine(blocLine);
                                }
                            }

                            //Case INSERT INTO
                            else if (line.StartsWith("INSERT INTO"))
                            {
                                translatedLine = translator.GenerateInsert(line);
                                outputStream.WriteLine(translatedLine);
                            }

                            //Case DROP TABLE
                            else if (line.StartsWith("DROP TABLE"))
                            {
                                translatedLine = translator.GenerateDrop(line);
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
