using System;
using System.Collections.Generic;
using System.Text;

namespace SQLTranslator.Destination
{
    class DestinationFactory : AbstractDestinationFactory
    {

        public override IDestination CreatePostgreSQLDestination()
        {
            return new PostgreSQLDestination();
        }

    }
}
