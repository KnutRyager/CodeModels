using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Record;

public class RecordPropertyDeclarationFromSourceTests
{
    [Fact] public void DefaultGetProperty() => @"
record A
{
    int B { get; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultSetProperty() => @"
record A
{
    int B { set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultGetSetProperty() => @"
record A
{
    int B { get; set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultGetInitProperty() => @"
record A
{
    int B { get; init; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PublicDefaultGetSetProperty() => @"
record A
{
    public int B { get; set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void VirtualDefaultGetSetProperty() => @"
record A
{
    virtual int B { get; set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void AbsrtactGetSetProperty() => @"
abstract record A
{
    public abstract int B { get; set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void StaticDefaultGetSetProperty() => @"
record A
{
    static int B { get; set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultPrivateGetSetProperty() => @"
record A
{
    int B { private get; private set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultProtectedGetSetProperty() => @"
record A
{
    int B { protected get; protected set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void DefaultInternalGetSetProperty() => @"
record A
{
    int B { internal get; internal set; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void GetExpressionBodyProperty() => @"
record A
{
    int B => 1;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void GetBlockBodyProperty() => @"
record A
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
record A
{
    int B { set => a = value; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void SetBlockBodyProperty() => @"
record A
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
record A
{
    int B { get => 1; set => a = value; }
}".AssertParsedAndGeneratedEqual();

    [Fact] public void GetAndSetBlockBodyProperty() => @"
record A
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