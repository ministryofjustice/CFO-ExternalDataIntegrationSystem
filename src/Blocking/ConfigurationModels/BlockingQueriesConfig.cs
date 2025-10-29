using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocking.ConfigurationModels
{
    public class BlockingQueriesConfig
    {
        //Blocking fields in the same inner array will be blocked on together (they will all need to match to be considered a candidate).
        public BlockingQueriesGroup[] BlockingQueriesGroups { get; init; } = [];
    }
}
