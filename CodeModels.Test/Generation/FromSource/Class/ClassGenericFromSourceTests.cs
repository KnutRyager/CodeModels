using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Class;

public class ClassGenericFromSourceTests
{
    [Fact] public void ClassGeneric() => @"
class A<T>
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassGenericWithClassConstraint() => @"
class A<T>
    where T : class
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassGenericWithNewConstraint() => @"
class A<T>
    where T : new()
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassGenericWithIntConstraint() => @"
class A<T>
    where T : int
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassGenericWithAnotherClassConstraint() => @"
class A
{
}

class B<T>
    where T : A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassGenericTwoParameters() => @"
class A<T1, T2>
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassGenericWithProperty() => @"
class A<T>
{
    T B { get; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassGenericWithMethod() => @"
class A<T>
{
    T B();
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassWithGenericMethod() => @"
class A
{
    T B<T>();
}".AssertParsedAndGeneratedEqual();
}