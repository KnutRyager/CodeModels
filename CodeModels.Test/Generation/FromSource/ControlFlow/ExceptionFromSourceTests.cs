using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Pattern;

public class ExceptionFromSourceTests
{
    [Fact] public void TryCatch() => @"
try
{
    1;
}
catch (Exception)
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void TryCatchExceptionInVariable() => @"
try
{
    1;
}
catch (Exception e)
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void TryCatchWhen() => @"
try
{
    1;
}
catch (Exception e)when (e is ArgumentException)
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void TryFinally() => @"
try
{
    1;
}
finally
{
    2;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void TryCatchFinally() => @"
try
{
    1;
}
catch (Exception e)
{
    2;
}
finally
{
    3;
}".AssertParsedAndGeneratedEqual();

    [Fact(Skip = "Not implemented")] public void Throw() => @"
throw new System.Exception();".AssertParsedAndGeneratedEqual();
}