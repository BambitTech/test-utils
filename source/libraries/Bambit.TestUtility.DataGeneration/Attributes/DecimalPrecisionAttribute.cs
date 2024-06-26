using System;

namespace Bambit.TestUtility.DataGeneration.Attributes
{
   
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DecimalPrecisionAttribute : Attribute
    {
        public byte Precision { get; set; }
        public byte Scale { get; set; }

        public DecimalPrecisionAttribute(byte precision, byte scale)
        {
            Precision = precision;
            Scale = scale;
        }
    }
}
