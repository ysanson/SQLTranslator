using System;
using System.Collections.Generic;
using System.Text;

namespace SQLTranslator.Destination
{
    public abstract class AbstractDestinationFactory
    {

        public abstract IDestination CreatePostgreSQLDestination();
    }
}
