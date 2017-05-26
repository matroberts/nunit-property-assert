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

        public class TestObject3PropertyDifferentType
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
            public float FloatProperty { get; set; }
        }

        public class TestObject3PropertyWrongPropertyTypes
        {
            public int StringProperty { get; set; }
            public float IntProperty { get; set; }
            public string FloatProperty { get; set; }
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
        public void PropertiesEqual_PropertiesCanBeIgnored_WithStringNamedProperties()
        {
            Assert.That(new TestObject1Property(), Properties.Equal(new TestObject3Property()).Ignore("IntProperty").Ignore("FloatProperty"));
        }

        [Test]
        public void PropertiesEqual_PropertiesCanBeIgnored_WithExpressionSyntax()
        {
            Assert.That(new TestObject1Property(), Properties.Equal(new TestObject3Property()).Ignore(o => o.IntProperty).Ignore(o => o.FloatProperty));
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
        public void PropertiesEqual_IfThePropertyValues_AreEqual_NoErrorReturned()
        {
            Assert.That(new TestObject1Property() { StringProperty = "right" }, Properties.Equal(new TestObject1Property() { StringProperty = "right" }));
        }

        [Test]
        public void PropertiesEqual_IfThePropertyValues_AreNotEqual_ReturnsError()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new TestObject1Property() {StringProperty = "wrong"}, 
                                                        Properties.Equal(new TestObject1Property() {StringProperty = "right"})));
            Assert.That(ex.Message, Contains.Substring("StringProperty"));
            Assert.That(ex.Message, Contains.Substring("Expected: \"right\""));
            Assert.That(ex.Message, Contains.Substring("But was:  \"wrong\""));
        }

        [Test]
        public void PropertiesEqual_IfAnyOfThePropertiesDiffer_AnErrorIsReturned()
        {
            Assert.Throws<AssertionException>(() => Assert.That(new TestObject3Property() { StringProperty = "right", FloatProperty = 7.0f, IntProperty = 9 },
                                               Properties.Equal(new TestObject3Property() { StringProperty = "right", FloatProperty = 2.0f, IntProperty = 9 })));
        }

        [Test]
        public void PropertiesEqual_TestsAndReportsOnAllOfTheProperties_EvenWithMultipleFailures()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new TestObject3Property() { StringProperty = "wrong", FloatProperty = 7.0f, IntProperty = 9 }, 
                                                        Properties.Equal(new TestObject3Property() { StringProperty = "right", FloatProperty = 2.0f, IntProperty = 11 })));
            Assert.That(ex.Message, Contains.Substring("StringProperty"));
            Assert.That(ex.Message, Contains.Substring("FloatProperty"));
            Assert.That(ex.Message, Contains.Substring("IntProperty"));
        }

        [Test]
        public void PropertiesEqual_TheObjectsDoNotHaveToBeTheSameType()
        {
            Assert.That(new TestObject3PropertyDifferentType() { StringProperty = "wrong", FloatProperty = 7.0f, IntProperty = 9 },
                    Properties.Equal(new TestObject3Property() { StringProperty = "wrong", FloatProperty = 7.0f, IntProperty = 9 }));
        }

        [Test]
        public void PropertiesEqual_UsesNUnitEqualComparisionOnTheProperties_WhenDeterminingIfPropertyTypesAreCompatible()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new TestObject3PropertyWrongPropertyTypes() { StringProperty = 2, FloatProperty = "string", IntProperty = 9 },
                         Properties.Equal(new TestObject3Property() { StringProperty = "wrong", FloatProperty = 7.0f, IntProperty = 9 })));
            Assert.That(ex.Message, Contains.Substring("StringProperty"));
            Assert.That(ex.Message, Contains.Substring("FloatProperty"));
            Assert.That(ex.Message, Contains.Substring("IntProperty"));
        }

        [Test]
        public void PropertiesEqual_IfExpectedAndActualAreNull_NoErrorReturned()
        {
            TestObject1Property expected = null;
            TestObject1Property actual = null;
            Assert.That(actual, Properties.Equal(expected));
        }

        [Test]
        public void PropertiesEqual_IfExpectedIsNull_ErrorReturned()
        {
            TestObject1Property expected = null;
            TestObject1Property actual = new TestObject1Property();
            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Properties.Equal(expected)));
            Assert.That(ex.Message, Contains.Substring("Expected is null"));
        }

        [Test]
        public void PropertiesEqual_IfActualIsNull_ErrorReturned()
        {
            TestObject1Property expected = new TestObject1Property();
            TestObject1Property actual = null;
            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Properties.Equal(expected)));
            Assert.That(ex.Message, Contains.Substring("Actual is null"));
        }
    }
}