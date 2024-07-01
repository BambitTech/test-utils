using System;

namespace Bambit.TestUtility.DataGeneration.Attributes
{
   
    /// <summary>
    /// Allows specification of precision and scale of a decimal
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DecimalPrecisionAttribute : Attribute
    {
        /// <summary>
        /// The precision of a decimal
        /// </summary>
        public byte Precision { get; set; }
        /// <summary>
        /// The magnitude of a decimal
        /// </summary>
        public byte Scale { get; set; }

        /// <summary>
        /// Creates a new DecimalPrecisionAttribute with the specified precision and scale
        /// </summary>
        /// <param name="precision">The <see cref="Precision"/></param>
        /// <param name="scale">The <see cref="Scale"/></param>
        public DecimalPrecisionAttribute(byte precision, byte scale)
        {
            Precision = precision;
            Scale = scale;
        }
    }
}
