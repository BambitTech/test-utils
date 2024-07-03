using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Bambit.TestUtility.DataGeneration.Attributes
{
   

    [TestClass]
    public class DecimalPrecisionAttributeTest
    {
        [TestMethod]
        public void Ctor_SetsPrecisionAsExpected()
        {
            for (byte x = 0; x < byte.MaxValue; x++)
            {
                DecimalPrecisionAttribute attribute = new DecimalPrecisionAttribute(x, 0);
                attribute.Precision.Should().Be(x);
            }
        }
        
        [TestMethod]
        public void Ctor_SetsScaleAsExpected()
        {
            for (byte x = 0; x < byte.MaxValue; x++)
            {
                DecimalPrecisionAttribute attribute = new DecimalPrecisionAttribute(0,x);
                attribute.Scale.Should().Be(x);
            }
        }


        [TestMethod]
        public void Precision_Setter_Sets()
        {
            byte testValue = 23;
            DecimalPrecisionAttribute attribute = new DecimalPrecisionAttribute(0, 0);
            attribute.Precision = testValue;
            attribute.Precision.Should().Be(testValue);

        }

        [TestMethod]
        public void Scale_Setter_Sets()
        {
            byte testValue = 101;
            DecimalPrecisionAttribute attribute = new DecimalPrecisionAttribute(0, 0);
            attribute.Scale = testValue;
            attribute.Scale.Should().Be(testValue);

        }

    }
}
