using CodeAnalyzation.Models.Execution;
using FluentAssertions;
using Xunit;

namespace Generator.Test.Models.Execution.Statement;

public class ConsoleTests
{
    [Fact] public void ConsoleWrite() => @"
System.Console.Write(""abc"");".Eval().Should().Be("abc");

    [Fact] public void ConsoleWriteConvertIntToString() => @"
System.Console.Write(4+5);".Eval().Should().Be("9");

    [Fact] public void ConsoleWriteLine() => @"
System.Console.WriteLine(""abc"");".Eval().Should().Be("abc\r\n");

    [Fact] public void ConsoleWriteMultiple() => @"
System.Console.Write(""a"");
System.Console.Write(""b"");".Eval().Should().Be("ab");

    [Fact] public void ConsoleWriteLineMultiple() => @"
System.Console.WriteLine(""a"");
System.Console.WriteLine(""b"");".Eval().Should().Be("a\r\nb\r\n");

    [Fact] public void ConsoleWriteLoop() => @"
for(var i = 0; i < 5; i++){
    System.Console.Write(i);
}".Eval().Should().Be("01234");

    [Fact] public void ConsoleWriteLineLoop() => @"
for(var i = 0; i < 5; i++){
    System.Console.WriteLine(i);
}".Eval().Should().Be("0\r\n1\r\n2\r\n3\r\n4\r\n");
}
