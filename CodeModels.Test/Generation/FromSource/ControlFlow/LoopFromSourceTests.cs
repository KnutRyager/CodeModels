using TestCommon;
using Xunit;

namespace CodeModels.Test.Generation.FromSource.Pattern;

public class LoopFromSourceTests
{
    [Fact(Skip = "Not implemented")] public void While() => @"
while (true) 1;".AssertParsedAndGeneratedEqual();

    [Fact] public void WhileBrackets() => @"
while (true)
{
    1;
}".AssertParsedAndGeneratedEqual();
    [Fact] public void DoWhile() => @"
do
{
    1;
}
while (true);".AssertParsedAndGeneratedEqual();

    [Fact(Skip = "Not implemented")] public void For() => @"
for (int i = 0; i < 10; i++) 1;".AssertParsedAndGeneratedEqual();

    [Fact] public void ForBrackets() => @"
for (int i = 0; i < 10; i++)
{
    1;
}".AssertParsedAndGeneratedEqual();

    [Fact(Skip = "Not implemented")] public void ForEach() => @"
foreach (var x in a) 1;".AssertParsedAndGeneratedEqual();

    [Fact] public void ForEachBrackets() => @"
foreach (var x in a)
{
    1;
}".AssertParsedAndGeneratedEqual();
}