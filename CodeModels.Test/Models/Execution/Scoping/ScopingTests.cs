using System.IO;
using System;
using FluentAssertions;
using Xunit;
using CodeAnalyzation.Models.Execution.ControlFlow;

namespace CodeAnalyzation.Models.Execution.Scoping.Test;

public class ScopingTests
{
    [Fact] public void UsingStatement() => @"
var lines = ""a\nb\nc"";
var result = """";
using (var reader = new System.IO.StringReader(lines))
{
    string? item;
    do
    {
        item = reader.ReadLine();
        result += item;
    } while (item != null);
}".Eval().Should().Be("abc");
    [Fact] public void UsingDeclaration() => @"
var lines = ""a\nb\nc"";
var result = """";
using var reader = new System.IO.StringReader(lines);
string? item;
do
{
    item = reader.ReadLine();
    result += item;
} while (item != null);
".Eval().Should().Be("abc");

    [Fact]
    public void UsingStatementCloses() => @"
var lines = ""a\nb\nc"";
System.IO.StringReader reader;
using (reader = new System.IO.StringReader(lines))
{
    reader.ReadLine();
}
reader.ReadLine();
}".Eval(catchExceptions: true).Should().BeEquivalentTo(new ObjectDisposedException("", "Cannot read from a closed TextReader."),
       options => options.Excluding(x => x.TargetSite).Excluding(x => x.Source).Excluding(x => x.StackTrace));

    [Fact]
    public void UsingStatementWithDeclarationCloses() => @"
var lines = ""a\nb\nc"";
System.IO.StringReader reader;
using (var reader2 = new System.IO.StringReader(lines))
{
    reader = reader2;
    reader2.ReadLine();
}
reader.ReadLine();
}".Eval(catchExceptions: true).Should().BeEquivalentTo(new ObjectDisposedException("", "Cannot read from a closed TextReader."),
       options => options.Excluding(x => x.TargetSite).Excluding(x => x.Source).Excluding(x => x.StackTrace));
}
