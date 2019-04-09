using System;
using System.Collections.Generic;
using System.Text;

namespace SQLTranslator.Destination
{
    public abstract class AbstractDestinationFactory
    {
        public enum Destinations { PostgreSQL11, OracleSQL };

        public abstract ITranslator CreateSQLTranslator(Destinations destination);
    }
}
