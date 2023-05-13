using CodeModels.Models;
using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Pattern;

public class PatternFromSourceTests
{
    [Fact] public void Constant() => "a is 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void Parenthesized() => "a is (1);".AssertParsedAndGeneratedEqual();
    [Fact] public void Declaration() => "a is int n;".AssertParsedAndGeneratedEqual();
    [Fact] public void Discard() => "_ is 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void VarDeclaration() => "a is var n;".AssertParsedAndGeneratedEqual();
    [Fact] public void IsType() => "a is int;".AssertParsedAndGeneratedEqual();
    [Fact] public void GreaterThan() => "a is> 1;".AssertParsedAndGeneratedEqual(); // Formatter is just weird here
    [Fact] public void GreaterThanOrEqual() => "a is >= 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void LessThan() => "a is < 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void LessThanOrEqual() => "a is <= 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void Or() => "a is 1 or 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void And() => "a is 1 and 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void Not() => "a is not 1;".AssertParsedAndGeneratedEqual();
    [Fact] public void ListEmpty() => "a is[];".AssertParsedAndGeneratedEqual();
    [Fact] public void ListOf1() => "a is[1];".AssertParsedAndGeneratedEqual();
    [Fact(Skip = "Not implemented")] public void ListSlice() => "1 is [>2,..,int];".AssertParsedAndGeneratedEqual();
    [Fact] public void ListType() => "a is[int];".AssertParsedAndGeneratedEqual();
    [Fact] public void ListGreaterThan() => "a is[  > 0];".AssertParsedAndGeneratedEqual();
    [Fact(Skip = "Not implemented")] public void SwitchExpression() => "a switch { _ => 1 };".AssertParsedAndGeneratedEqual();
    [Fact(Skip = "Not implemented")] public void SwitchExpressionWith2Cases() => "a switch { > 4 => 1, _ => 1 };".AssertParsedAndGeneratedEqual();
    [Fact(Skip = "Not implemented")] public void PropertyPattern() => "a switch { A: 1 };".AssertParsedAndGeneratedEqual();
    [Fact(Skip = "Not implemented")] public void PropertyPatternRecursive() => "a switch { A.B.C: 1 };".AssertParsedAndGeneratedEqual();
}