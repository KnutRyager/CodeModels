using CodeModels.Models;
using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Pattern;

public class IfElseFromSourceTests
{
    [Fact(Skip = "Not implemented")] public void If() => @"if (true) 1;".AssertParsedAndGeneratedEqual();
    [Fact(Skip = "Not implemented")] public void IfElse() => @"if (true) 1 else 2;".AssertParsedAndGeneratedEqual();
    [Fact(Skip = "Not implemented")] public void IfElseIf() => @"if (true) 1 else if 2;".AssertParsedAndGeneratedEqual();
    [Fact] public void IfBrackets() => @"
if (true)
{
    1;
}".AssertParsedAndGeneratedEqual();
    [Fact] public void IfElseBracketes() => @"
if (true)
{
    1;
}
else
{
    2;
}".AssertParsedAndGeneratedEqual();
    [Fact] public void IfElseIfBracketes() => @"
if (true)
{
    1;
}
else if (false)
{
    2;
}".AssertParsedAndGeneratedEqual();
}