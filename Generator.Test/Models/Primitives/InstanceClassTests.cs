using FluentAssertions;
using Xunit;

namespace CodeAnalyzation.Models.Primitives.Test;

public class InstanceClassTests
{
    [Fact]
    public void GetPropertyAccessValue() => new InstanceClass("ClassA", new PropertyCollection(new Property[] {
            Property.FromValue(new QuickType("string"),"A",new LiteralExpression("B"), modifier: PropertyAndFieldTypes.PublicField),
            new(new QuickType("int"),"B"),
        })).GetProperty("A").AccessValue("abc").Syntax.ToString().Should().Equals("abc.A");
}
