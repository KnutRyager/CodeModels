using System.Data;
using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Class;

public class ClassPropertyDeclarationFromSourceTests
{
    [Fact] public void DefaultGetProperty() => @"
class A
{
    int B { get; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultSetProperty() => @"
class A
{
    int B { set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultGetSetProperty() => @"
class A
{
    int B { get; set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultGetInitProperty() => @"
class A
{
    int B { get; init; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PublicDefaultGetSetProperty() => @"
class A
{
    public int B { get; set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void VirtualDefaultGetSetProperty() => @"
class A
{
    virtual int B { get; set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AbsrtactGetSetProperty() => @"
abstract class A
{
    public abstract int B { get; set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void StaticDefaultGetSetProperty() => @"
class A
{
    static int B { get; set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultPrivateGetSetProperty() => @"
class A
{
    int B { private get; private set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultProtectedGetSetProperty() => @"
class A
{
    int B { protected get; protected set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultInternalGetSetProperty() => @"
class A
{
    int B { internal get; internal set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void GetExpressionBodyProperty() => @"
class A
{
    int B => 1;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void GetBlockBodyProperty() => @"
class A
{
    int B
    {
        get
        {
            int a = 1;
            return a;
        }
    }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void SetExpressionBodyProperty() => @"
class A
{
    int B { set => a = value; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void SetBlockBodyProperty() => @"
class A
{
    int B
    {
        set
        {
            a = value;
        }
    }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void GetAndSetExpressionBodyProperty() => @"
class A
{
    int B { get => 1; set => a = value; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void GetAndSetBlockBodyProperty() => @"
class A
{
    int B
    {
        get
        {
            return 1;
        }

        set
        {
            a = value;
        }
    }
}".AssertParsedAndGeneratedEqual();
}