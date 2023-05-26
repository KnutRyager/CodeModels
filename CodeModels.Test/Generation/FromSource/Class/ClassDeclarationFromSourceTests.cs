using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Class;

public class ClassDeclarationFromSourceTests
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

    [Fact] public void FileClass() => @"
file class A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PublicStaticClass() => @"
public static class A
{
}".AssertParsedAndGeneratedEqual();
}