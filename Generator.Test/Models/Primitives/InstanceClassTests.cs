using FluentAssertions;
using Xunit;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models.Primitives.Test;

public class InstanceClassTests
{
    [Fact]
    public void GetPropertyAccessValue() => new InstanceClass("ClassA", new PropertyCollection(new Property[] {
            Property(Type("string"),"A", Literal("B"), modifier: PropertyAndFieldTypes.PublicField),
            Property(Type("int"),"B"),
        })).GetProperty("A").AccessValue("abc").Syntax().ToString().Should().Equals("abc.A");
}
