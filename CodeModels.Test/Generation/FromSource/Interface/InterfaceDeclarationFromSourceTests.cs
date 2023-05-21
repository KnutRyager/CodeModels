using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Interface;

public class InterfaceDeclarationFromSourceTests
{
    [Fact] public void Interface() => @"
interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PublicInterface() => @"
public interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void StaticInterface() => @"
static interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PrivateInterface() => @"
private interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ProtectedInterface() => @"
protected interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InternalInterface() => @"
internal interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PublicStaticInterface() => @"
public static interface A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfacePropertyGet() => @"
interface A
{
    int B { get; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfacePropertySet() => @"
interface A
{
    int B { set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfacePropertyInit() => @"
interface A
{
    int B { init; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfacePropertyGetSet() => @"
interface A
{
    int B { get; set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InterfaceMethod() => @"
interface A
{
    int B();
}".AssertParsedAndGeneratedEqual();
}