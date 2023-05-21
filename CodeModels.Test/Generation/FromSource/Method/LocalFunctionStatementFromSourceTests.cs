using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Interface;

public class LocalFunctionStatementFromSourceTests
{
    [Fact] public void EmptyLocalFunction() => @"
int F()
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void LocalFunctionWithBody() => @"
int F()
{
    return 0;
}".AssertParsedAndGeneratedEqual();

    [Fact] public void LocalFunctionWithExpressionBody() => @"
int F() => 0;".AssertParsedAndGeneratedEqual();

    [Fact] public void LocalFunctionWithParameter() => @"
int F(int a)
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void LocalFunctionWithTwoParameters() => @"
int F(int a, int b)
{
}".AssertParsedAndGeneratedEqual();

    [Fact] public void GenericLocalFunction() => @"
int F<T>() => 0;".AssertParsedAndGeneratedEqual();

    [Fact] public void GenericLocalFunctionWithGenericParameter() => @"
int F<T>(T a) => 0;".AssertParsedAndGeneratedEqual();

    [Fact] public void GenericLocalFunctionWithGenericReturn() => @"
T F<T>() => null;".AssertParsedAndGeneratedEqual();

    [Fact] public void GenericLocalFunctionTwoParameters() => @"
int F<T, U>() => 0;".AssertParsedAndGeneratedEqual();

    [Fact] public void GenericLocalFunctionWithClassConstraint() => @"
int F<T>()
    where T : class => 0;".AssertParsedAndGeneratedEqual();

    [Fact] public void GenericLocalFunctionWithIntConstraint() => @"
int F<T>()
    where T : int => 0;".AssertParsedAndGeneratedEqual();

    [Fact] public void GenericLocalFunctionWithNewConstraint() => @"
int F<T>()
    where T : new() => 0;".AssertParsedAndGeneratedEqual();

    [Fact] public void GenericLocalFunctionWithNotnullConstraint() => @"
int F<T>()
    where T : notnull => 0;".AssertParsedAndGeneratedEqual();
}