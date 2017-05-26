using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Constraints;

namespace NUnitPropertyAssert
{
    public class PropertiesEqualConstraint : Constraint
    {
        private readonly object expected;
        private List<string> propertiesToIgnore = new List<string>();

        public PropertiesEqualConstraint(object expected)
        {
            this.expected = expected;
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            // actual should contain all of the properties which expected has
            // though actual may have extra properties which expected does not
            var expectedProperties = new List<PropertyInfo>(expected.GetType().GetProperties());
            var actualProperties = new List<PropertyInfo>(actual.GetType().GetProperties());

            var missingProperties = expectedProperties.Select(p => p.Name).Except(actualProperties.Select(p => p.Name)).Except(propertiesToIgnore).ToList();
            if(missingProperties.Any())
                return new PropertiesEqualConstraintResult(this, actual, false, $"Expected contains the following properties which actual is missing: {string.Join(", ", missingProperties)}");


            // loop though expected objects properties
            // and do an nunit areequal on each property
            // 

            return new PropertiesEqualConstraintResult(this, actual, true, $"Expected and Actual are have equal properties.");
        }

        public PropertiesEqualConstraint Ignore(string propertyName)
        {
            propertiesToIgnore.Add(propertyName);
            return this;
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

    public class Properties 
    {
        public static PropertiesEqualConstraint Equal(object expected)
        {
            return new PropertiesEqualConstraint(expected);
        }
    }
}