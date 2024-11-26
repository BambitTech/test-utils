using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow
{
    public class MockedObject
    {
        public string ConnectionName { get; set; }
        public string OriginalSchema { get; set; }
        public string OriginalName { get; set; }
        public string NewName { get; set; }
        public List<string> RestoreScripts { get; set; }
        public DatabseObjectType DatabseObjectType { get; set; }
    }
}
