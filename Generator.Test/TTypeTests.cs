using CodeAnalyzation.Models;
using FluentAssertions;
using Xunit;
using static CodeAnalysisTests.TestUtil;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Generator.Test;

public class TTypeTests
{

    [Fact]
    public void ParseInt() => TType.Parse("int").TypeSyntax().CodeEqual("int");
    [Fact]
    public void ParseOptionalInt() => TType.Parse("int?").TypeSyntax().CodeEqual("int?");
}
