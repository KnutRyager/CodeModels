using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Interface;

public class InterfaceGenericFromSourceTests
{
    [Fact] public void InterfaceGeneric() => @"
interface A<T>
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfaceGenericWithClassConstraint() => @"
interface A<T>
    where T : class
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfaceGenericWithNewConstraint() => @"
interface A<T>
    where T : new()
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfaceGenericWithIntConstraint() => @"
interface A<T>
    where T : int
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfaceGenericWithInterfaceConstraint() => @"
interface A
{
}

interface B<T>
    where T : A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfaceGenericTwoParameters() => @"
interface A<T1, T2>
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfaceGenericWithProperty() => @"
interface A<T>
{
    T B { get; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfaceGenericWithMethod() => @"
interface A<T>
{
    T B();
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfaceWithGenericMethod() => @"
interface A
{
    T B<T>();
}".AssertParsedAndGeneratedEqual();
}