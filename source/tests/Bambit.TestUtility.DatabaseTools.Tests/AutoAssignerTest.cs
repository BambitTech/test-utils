using Bambit.TestUtility.DataGeneration;
using Bambit.TestUtility.TestHelper;
using FluentAssertions;

namespace Bambit.TestUtility.DatabaseTools.Tests
{
    [TestClass]
    public class AutoAssignerTest
    {
        [TestMethod]
        public void AssignValuesIfDefined_ClassHasChildObjects_DictionaryHasLevels_AssignsChild()
        {
            TestClass testClass = new TestClass();
            Dictionary<string, string?> properties = new Dictionary<string, string?>();
            TestChildClass childClass = RandomDataGenerator.Instance.InitializeObject<TestChildClass>(r =>
                r.TestGrandChild =
                    RandomDataGenerator.Instance.InitializeObject<TestGrandChild>());
            properties.Add("ChildClass.ChildName", childClass .ChildName);
            properties.Add("childClass.SubName", childClass.SubName);
            properties.Add("ChildClass.BoolField", childClass.BoolField.ToString());
            properties.Add("ChildClass.TestGrandChild.GrandChildName", childClass.TestGrandChild.GrandChildName);
            properties.AssignValuesIfDefined(testClass);
            testClass.ChildClass.Should().BeEquivalentTo(childClass);
        }
    }
}
