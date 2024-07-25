using Bambit.TestUtility.DatabaseTools;
using Bambit.TestUtility.DatabaseTools.Attributes;

namespace Bambit.TestUtility.TestHelper
{
    public class MappedClassInstance : DatabaseMappedClass
    {
        public MappedClassInstance() { }

        
        public string? Name { get; set; }

        public int Age { get; set; }

        public decimal Height { get; set; }

        public DateTime DateOfBirth { get; set; }
    }

    public  class MappedClassWithComputedColumn : MappedClassInstance
    {
        [ComputedColumn]
        public string? WeekDay { get; set; }
    }
}
