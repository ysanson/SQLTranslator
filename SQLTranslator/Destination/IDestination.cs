using System;
using System.Collections.Generic;
using System.Text;

namespace SQLTranslator.Destination
{
    public interface IDestination
    {
        List<string> GenerateTable(List<string> inputBloc);

        string GenerateInsert(string inputLine);

        string GenerateDrop(string inputLine);
    }
}
