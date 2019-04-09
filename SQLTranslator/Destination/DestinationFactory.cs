using System;
using System.Collections.Generic;
using System.Text;

namespace SQLTranslator.Destination
{
    class DestinationFactory : AbstractDestinationFactory
    {
        /// <summary>
        /// Creates a new SQL destination.
        /// </summary>
        /// <param name="destination">The translator to create.</param>
        /// <returns>A translator. By default it creates a Postgres translator.</returns>
        public override ITranslator CreateSQLTranslator(Destinations destination)
        {
            switch (destination)
            {
                case Destinations.PostgreSQL11:
                    return new PostgreSQLTranslator();
                case Destinations.OracleSQL:
                    throw new NotImplementedException();

                default:
                    return new PostgreSQLTranslator();
                
            }
        }
    }
}
