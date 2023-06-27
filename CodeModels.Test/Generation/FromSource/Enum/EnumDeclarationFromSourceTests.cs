using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Enum;

public class EnumDeclarationFromSourceTests
{
    [Fact] public void Enum() => @"
enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PublicEnum() => @"
public enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void StaticEnum() => @"
static enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PrivateEnum() => @"
private enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void ProtectedEnum() => @"
protected enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void InternalEnum() => @"
internal enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void FileEnum() => @"
file enum A
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void PublicStaticEnum() => @"
public static enum A
{
}".AssertParsedAndGeneratedEqual();
}