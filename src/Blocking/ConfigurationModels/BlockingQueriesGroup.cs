using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blocking.ConfigurationModels
{
    public class BlockingQueriesGroup
    {
        public int Order { get; set; }
        public string BlockingQuery { get; set; } = string.Empty;
    }
}
