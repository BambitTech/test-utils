using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Bambit.TestUtility.TestHelper
{
    [ExcludeFromCodeCoverage]
    public class TestClass
    {
        public string FirstName { get; set; } = null!;
        public int IntField { get; set; }

        public byte ByteField { get; set; }
        public double DoubleField { get; set; }
        public float FloatField { get; set; }

        public short ShortField { get; set; }
        public DateTime DateTimeField { get; set; }
        public DateTimeOffset DateTimeOffsetField { get; set; }
        public long LongField { get; set; }
        public decimal DecimalField { get; set; }
        public Guid GuidField { get; set; }
        public bool BoolField { get; set; }


        
        public string? NullableFirstName { get; set; } = null!;
        public int? NullableIntField { get; set; }
        public byte? NullableByteField { get; set; }
        public double? NullableDoubleField { get; set; }
        public float? NullableFloatField { get; set; }

        public short? NullableShortField { get; set; }
        public DateTime? NullableDateTimeField { get; set; }
        public DateTimeOffset? NullableDateTimeOffsetField { get; set; }
        public long ? NullableLongField { get; set; }
        public decimal? NullableDecimalField { get; set; }
        public Guid? NullableGuidField { get; set; }
        public bool? NullableBoolField { get; set; }



        public ITestChildClass ChildClassInterface { get; set; } = null!;
        public TestChildClass ChildClass { get; set; } = null!;

        public TestChildClass[] Children { get; set; } = [];
    }

    public interface ITestChildClass
    {
        string ChildName { get; set; }
        string SubName { get; set; }
        bool BoolField { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class TestChildClass : ITestChildClass
    {
        [StringLength(4)]
        public string ChildName { get; set; } = null!;
        
        [MaxLength(6)]
        public string SubName { get; set; } = null!;
        public bool BoolField { get; set; }

        public TestGrandChild TestGrandChild
        {
            get;
            set;
        } = null!;
    }
    [ExcludeFromCodeCoverage]
    
    public class TestGrandChild 
    {
        public string GrandChildName { get; set; } = null!;
     
    }
    [ExcludeFromCodeCoverage]
    public class RecursiveClass
    {
        public RecursiveClass Child { get; set; } = null!;
    }

    [ExcludeFromCodeCoverage]
    public class NoDefaultCtorClass(int v)
    {
        public int IntField { get; set; } = v;
    }
    public enum TestEnum
    {
        Freddy,
        Jason,
        Michael,
        Dracula,
        Adam
        
    }
}
