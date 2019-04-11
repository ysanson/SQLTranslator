using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SQLTranslator.Destination
{
    /// <summary>
    /// The translator to PostgreSQL.
    /// </summary>
    class PostgreSQLTranslator : ITranslator
    {
        
        public List<string> GenerateTable(List<string> inputBloc)
        {
            List<string> tableBody = new List<string>();
            string newDeclaration, tableName = "";
            List<string> beforeStatements = new List<string> { string.Empty };
            List<string> afterStatements = new List<string>();


            foreach (string line in inputBloc)
            {
                newDeclaration = line;
                if (newDeclaration.Contains("CREATE TABLE"))
                {

                    string[] splitted = newDeclaration.Split(" ");
                    tableName = splitted[2].Replace("\"", string.Empty);
                }
                else
                    newDeclaration = newDeclaration.ToUpper();
                //Types treatments

                //If it's an auto increment, it's probably a primary key.
                if (newDeclaration.Contains("AUTO_INCREMENT"))
                {
                    newDeclaration = Regex.Replace(newDeclaration, @"INT\((\d+)\)", "SERIAL");
                    newDeclaration = newDeclaration.Replace(" AUTO_INCREMENT", string.Empty);
                }
                //If it is an Integer (the space is here to differenciate between other types)
                else if (newDeclaration.Contains(" INT"))
                {
                    newDeclaration = Regex.Replace(newDeclaration, @"INT\((\d+)\)", "INTEGER");
                    newDeclaration = RemoveDefaultValueForNumericTypes(newDeclaration);

                }
                //If it's a varchar
                else if (newDeclaration.Contains("VARCHAR"))
                {
                    if (newDeclaration.Contains(" DEFAULT"))
                        newDeclaration = RemoveDefaultValueForCharacterTypes(newDeclaration);
                }
                //It it is an enumeration, we have to create a special type for it.
                else if (newDeclaration.Contains("ENUM"))
                {
                    string[] enums = newDeclaration.TrimStart().Split(" "); //0: property name 1: ENUM(...) 2: (NOT) 3: NULL 4: DEFAULT 5: def Value
                    string dropType = "DROP TYPE IF EXISTS" + enums[0].Trim('"') + "_type";
                    string enumeration = "CREATE TYPE " + enums[0].Trim('"') + "_type AS " + enums[1] + (";");
                    beforeStatements.Add(enumeration);

                    newDeclaration = "  " + enums[0] + " " + enums[0].Trim('"') + "_type " + enums[2];
                    if (enums[2] == "NOT")
                        newDeclaration += " NULL,";
                    else
                        newDeclaration += ",";
                }
                //If it is a tiny int, we convert it to a smallint.
                else if (newDeclaration.Contains(" TINYINT"))
                {
                    newDeclaration = Regex.Replace(newDeclaration, @"TINYINT\((\d+)\)", "SMALLINT");
                    newDeclaration = RemoveDefaultValueForNumericTypes(newDeclaration);
                }
                else if (newDeclaration.Contains("SMALLINT"))
                {
                    newDeclaration = Regex.Replace(newDeclaration, @"SMALLINT\((\d+)\)", "SMALLINT");
                    newDeclaration = RemoveDefaultValueForNumericTypes(newDeclaration);
                }
                //If it is a float, we convert it.
                else if (newDeclaration.Contains("FLOAT"))
                {
                    newDeclaration = Regex.Replace(newDeclaration, @"FLOAT\((\d+,\d+)\)", "FLOAT");
                    newDeclaration = newDeclaration.Replace("FLOAT", "REAL");
                    newDeclaration = RemoveDefaultValueForNumericTypes(newDeclaration);
                }
                //If it is a char, we convert it to a varchar with the same precision.
                else if (newDeclaration.Contains(" CHAR"))
                {
                    newDeclaration = newDeclaration.Replace(" CHAR", " VARCHAR");
                    newDeclaration = RemoveDefaultValueForCharacterTypes(newDeclaration);
                }
                else if (newDeclaration.Contains("DATE"))
                {
                    if (newDeclaration.Contains("NOT NULL"))
                    {
                        string[] args = newDeclaration.TrimStart().Split(" "); //0: name 1: DATE 2:NOT 3: NULL 4: DEFAULT 5: def_value
                        newDeclaration = "  " + args[0] + " DATE NOT NULL DEFAULT now(),";

                    }
                }
                //Contstraints treatments

                /*
                if(newDeclaration.Contains("PRIMARY KEY"))
                {
                    
                }
                */
                else if (newDeclaration.Contains("UNIQUE KEY"))
                {
                    string[] keys = newDeclaration.TrimStart().Split(" "); //0: UNIQUE 1: KEY 2: KEY_NAME 3:KEY_PROPS
                    newDeclaration = "  UNIQUE " + keys[3];
                }
                else if (newDeclaration.TrimStart().StartsWith("KEY"))
                {
                    string[] args = newDeclaration.TrimStart().Split(' '); //Case 0: KEY; 1: index name, 2: index arguments
                    string index;
                    if (args[2].EndsWith(','))
                        index = "CREATE INDEX " + tableName + "_" + args[1].Trim('"') + " ON " + tableName + args[2].Substring(0, args[2].Length - 1) + ";";
                    else
                        index = "CREATE INDEX " + tableName + "_" + args[1].Trim('"') + " ON " + tableName + args[2] + ";";
                    afterStatements.Add(index);
                    continue;
                }

                //Final newDeclaration of the table creation
                if (newDeclaration.StartsWith(')'))
                {
                    newDeclaration = ");";
                }

                tableBody.Add(newDeclaration);
            }

            if (tableBody[tableBody.Count - 2].EndsWith(','))
                tableBody[tableBody.Count - 2] = tableBody[tableBody.Count - 2].Substring(0, tableBody[tableBody.Count - 2].Length - 1);

            beforeStatements.AddRange(tableBody);
            beforeStatements.AddRange(afterStatements);
            beforeStatements.Add(string.Empty);
            return beforeStatements;
        }

        private string RemoveDefaultValueForNumericTypes(string line)
        {
            if (line.Contains("DEFAULT") && line.Contains("NOT NULL"))//If the int is not nullable, we provide a default value.
            {
                string defaultValue = Regex.Match(line.Substring(line.LastIndexOf('T')), @"\d+").Value;
                line = Regex.Replace(line, @"'\d+'", defaultValue);
            }
            else if (line.Contains("DEFAULT") && !line.Contains("NOT NULL")) //If it is nullable, we suppress the default keyword.
            {
                line = line.Replace("DEFAULT", string.Empty);
                line = Regex.Replace(line, @"'\d+'", string.Empty);
                if (!line.Contains("NULL"))
                {
                    line = line.Remove(line.Length - 1);
                    line += " NULL,";
                }
            }
            return line;
        }

        private string RemoveDefaultValueForCharacterTypes(string line)
        {
            if (!line.Contains("NOT NULL") || line.Contains("''"))
            {
                line.Replace("DEFAULT ''", string.Empty);
            }
            //If the varchar is nullable with a default value, we keep it.
            if (Regex.IsMatch(line.Substring(line.LastIndexOf('T')), @"'\.+'"))
            {
                string defaultValue = line.Substring(line.LastIndexOf('T'));
                defaultValue.Substring(defaultValue.IndexOf('\''), defaultValue.Length - defaultValue.LastIndexOf('\''));
                line = Regex.Replace(line, @"'\d+'", defaultValue.ToLower());
            }

            return line;
        }

        public string GenerateInsert(string inputLine)
        {
            string output = inputLine;
            output = output.Replace("\\'", "''");
            return output;
        }

        public string GenerateDrop(string inputLine)
        {
            string output;
            if(!inputLine.Contains("CASCADE"))
            {
                output = inputLine.TrimEnd(';');
                output += " CASCADE;";
            }
            else
                output = inputLine;
            return output;
        }
    }

}

