using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NUnitPropertyAssert
{
    [TestFixture]
    public class PropertiesEqualConstraintTests
    {
        public class TestObject0Property
        {
            
        }

        public class TestObject1Property
        {
            public string StringProperty { get; set; }
        }

        public class TestObject1PrivateProperty
        {
            private string StringProperty { get; set; }
        }

        public class TestObject2Property
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
        }
        public class TestObject3Property
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
            public float FloatProperty { get; set; }
        }

        [Test]
        public void PropertiesEqual_IfTheActualObjectHasSamePropertiesAndValues_AsExpectedObject_NoErrorReturned()
        {
            Assert.That(new TestObject1Property(), Properties.Equal(new TestObject1Property()));
        }

        [Test]
        public void PropertiesEqual_IfTheActualPropertied_HasPropertiesWhichTheExpectedDoesNot_NoErrorReturned()
        {
            Assert.That(new TestObject3Property(), Properties.Equal(new TestObject1Property()));
        }

        [Test]
        public void PropertiesEqual_IfTheActualObjectIsMissingProperties_WhichExpectedObjectContains_ReturnsError()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new TestObject1Property(), Properties.Equal(new TestObject3Property())));
            Assert.That(ex.Message, Contains.Substring("Expected contains the following properties which actual is missing: IntProperty, FloatProperty"));
        }

        [Test]
        public void PropertiesEqual_PropertiesCanBeIgnored()
        {
            Assert.That(new TestObject1Property(), Properties.Equal(new TestObject3Property()).Ignore("IntProperty").Ignore("FloatProperty"));
        }

        [Test]
        public void PropertiesEqual_PrivatePropertiesShouldBeIgnored_InTheActualObject()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new TestObject1PrivateProperty(), Properties.Equal(new TestObject1Property())));
            Assert.That(ex.Message, Contains.Substring("Expected contains the following properties which actual is missing: StringProperty"));
        }

        [Test]
        public void PropertiesEqual_PrivatePropertiesShouldBeIgnored_InTheExpectedObject()
        {
            Assert.That(new TestObject0Property(), Properties.Equal(new TestObject1PrivateProperty()));
        }

        [Test]
        public void PropertiesEqual_IfThePropertyValues_AreNotEqual_ReturnsError()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new TestObject1Property() {StringProperty = "wrong"}, Properties.Equal(new TestObject1Property() {StringProperty = "right"})));
            Assert.That(ex.Message, Contains.Substring("StringProperty"));
            Assert.That(ex.Message, Contains.Substring("Expected: \"right\""));
            Assert.That(ex.Message, Contains.Substring("But was:  \"wrong\""));
        }

        [Test]
        public void PropertiesEqual_TestsAndReportsOnAllOfTheProperties_EvenWithMultipleFailures()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new TestObject3Property() { StringProperty = "wrong", FloatProperty = 7.0f, IntProperty = 9 }, Properties.Equal(new TestObject3Property() { StringProperty = "right", FloatProperty = 2.0f, IntProperty = 11 })));
            Assert.That(ex.Message, Contains.Substring("StringProperty"));
            Assert.That(ex.Message, Contains.Substring("FloatProperty"));
            Assert.That(ex.Message, Contains.Substring("IntProperty"));
        }

        [Test]
        public void PropertiesEqual_IfThePropertyValues_AreEqual_NoErrorReturned()
        {
            Assert.That(new TestObject1Property() { StringProperty = "right" }, Properties.Equal(new TestObject1Property() { StringProperty = "right" }));
        }
    }
}