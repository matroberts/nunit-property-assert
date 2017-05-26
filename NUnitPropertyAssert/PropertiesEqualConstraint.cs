using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework.Constraints;

namespace NUnitPropertyAssert
{
    public class Properties
    {
        public static PropertiesEqualConstraint<TExpected> Equal<TExpected>(TExpected expected)
        {
            return new PropertiesEqualConstraint<TExpected>(expected);
        }
    }

    public class PropertiesEqualConstraint<TExpected> : Constraint
    {
        private readonly TExpected expected;
        private readonly List<string> propertiesToIgnore = new List<string>();

        public PropertiesEqualConstraint(TExpected expected)
        {
            this.expected = expected;
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            // null checks mimic how nunit equals work
            if(expected==null && actual==null)
                return new ConstraintResult(this, actual, true);
            if(expected==null)
                return new MessageConstraintResult(this, actual, "Expected is null");
            if (actual == null)
                return new MessageConstraintResult(this, actual, "Actual is null");

            // actual should contain all of the properties which expected has
            // though actual may have extra properties which expected does not
            var expectedProperties = new List<PropertyInfo>(expected.GetType().GetProperties().Where(p => propertiesToIgnore.Contains(p.Name) == false));
            var actualProperties = new List<PropertyInfo>(actual.GetType().GetProperties());

            var missingProperties = expectedProperties.Select(p => p.Name).Except(actualProperties.Select(p => p.Name)).ToList();
            if(missingProperties.Any())
                return new MessageConstraintResult(this, actual, $"Expected contains the following properties which actual is missing: {string.Join(", ", missingProperties)}");

            // loop though expected objects properties, and use nunit equal constraint to see if properties are equal
            var result = new MultiEqualsConstraintResult(this, actual);
            foreach (var expectedProperty in expectedProperties)
            {
                var expectedPropertyValue = expectedProperty.GetValue(expected, null);
                var actualPropertyValue = actualProperties.Single(p => p.Name == expectedProperty.Name).GetValue(actual, null);

                var equalConstraint = new EqualConstraint(expectedPropertyValue);
                result.AddResult(expectedProperty.Name, equalConstraint.ApplyTo(actualPropertyValue));
            }

            return result;
        }

        public PropertiesEqualConstraint<TExpected> Ignore(string propertyName)
        {
            propertiesToIgnore.Add(propertyName);
            return this;
        }

        public PropertiesEqualConstraint<TExpected> Ignore<TProperty>(Expression<Func<TExpected, TProperty>> propertyNameExpression)
        {
            var memberExpression = propertyNameExpression.Body as MemberExpression;
            if (memberExpression != null)
            {
                propertiesToIgnore.Add(memberExpression.Member.Name);
            }
            return this;
        }
    }

    public class MessageConstraintResult : ConstraintResult
    {
        private readonly string message;

        public MessageConstraintResult(IConstraint constraint, object actualValue, string message) : base(constraint, actualValue, false)
        {
            this.message = message;
        }

        public override void WriteMessageTo(MessageWriter writer)
        {
            writer.WriteMessageLine(message);
        }
    }

    public class MultiEqualsConstraintResult : ConstraintResult
    {
        private readonly List<KeyValuePair<string, ConstraintResult>> results = new List<KeyValuePair<string, ConstraintResult>>();

        public MultiEqualsConstraintResult(IConstraint constraint, object actualValue) : base(constraint, actualValue)
        {
        }

        public void AddResult(string propertyName, ConstraintResult result)
        {
            results.Add(new KeyValuePair<string, ConstraintResult>(propertyName, result));
        }

        public override bool IsSuccess => results.All(r => r.Value.IsSuccess);

        public override void WriteMessageTo(MessageWriter writer)
        {
            foreach (var result in results.Where(r => r.Value.IsSuccess ==false))
            {
                writer.WriteMessageLine($"{result.Key} differ:");
                result.Value.WriteMessageTo(writer);
                writer.WriteLine();
            }
        }
    }

}