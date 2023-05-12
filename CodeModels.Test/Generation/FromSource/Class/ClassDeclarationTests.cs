using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Class;

public class GenerateClassFromSourceTests
{
    [Fact] public void Class() => @"
class A
{
}".AssertParsedAndGeneratedEqual();
    [Fact] public void PublicClass() => @"
public class A
{
}".AssertParsedAndGeneratedEqual();
    [Fact] public void StaticClass() => @"
static class A
{
}".AssertParsedAndGeneratedEqual();
    [Fact] public void PrivateClass() => @"
private class A
{
}".AssertParsedAndGeneratedEqual();
    [Fact] public void ProtectedClass() => @"
protected class A
{
}".AssertParsedAndGeneratedEqual();
    [Fact] public void InternalClass() => @"
internal class A
{
}".AssertParsedAndGeneratedEqual();
    [Fact] public void PublicStaticClass() => @"
public static class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ClassWithField() => @"
class A
{
    int B;
}".AssertParsedAndGeneratedEqual();
    [Fact] public void ClassWithPrivateField() => @"
class A
{
    private int B;
}".AssertParsedAndGeneratedEqual();
    [Fact] public void ClassWithFieldWithValue() => @"
class A
{
    int B = 1337;
}".AssertParsedAndGeneratedEqual();
}