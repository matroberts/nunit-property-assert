using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NUnitPropertyAssert
{
    public class PropertiesEqualConstraint : Constraint
    {
        private readonly object _expected;

        public PropertiesEqualConstraint(object expected)
        {
            _expected = expected;
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            if(_expected.GetType() != actual.GetType())
                return new PropertiesEqualConstraintResult(this, actual, false, $"Expected object of type {_expected.GetType()} actual object of type {actual.GetType()}");

            // loop though expected objects properties
            // and do an nunit areequal on each property
            // 

            throw new NotImplementedException();
        }
    }

    public class PropertiesEqualConstraintResult : ConstraintResult
    {
        private readonly string message;

        public PropertiesEqualConstraintResult(IConstraint constraint, object actualValue, bool isSuccess, string message) : base(constraint, actualValue, isSuccess)
        {
            this.message = message;
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            writer.WriteMessageLine(message);
        }
    }

    [TestFixture]
    public class NUnitPropertyAssert
    {
        public class MyObject
        {
            public string StringProperty { get; set; }
        }

        public class AnotherObject
        {
            public string AnotherStringProperty { get; set; }
        }



        [Test]
        public void PropertiesEqual_ReturnsFalse_IfTheTypesOfTheObjectsAreDifferent()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new MyObject(), new PropertiesEqualConstraint(new AnotherObject())));
            Console.WriteLine(ex.Message);
        }

        [Test]
        public void Test()
        {
            Assert.That(1, Is.EqualTo(1));

            //Assert.That(utilisation, Properties.EqualTo(expectedResult).Ignore(expectedResult.Id));
            //Assert.That(utilisation, Is.PropertiesEqualTo(expectedResult).Ignore(expectedResult.Id));
        }

    }
}