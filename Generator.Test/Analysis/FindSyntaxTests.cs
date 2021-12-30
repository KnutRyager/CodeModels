using System.Linq;
using CodeAnalyzation;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace Generator.Test.Analysis;

public class FindSyntaxTests
{
    [Fact]
    public void GetPublicStaticFields()
        => Assert.Equal("public const DateTime variable = null;",
            "using System; class Test{public const DateTime variable = null;}".Parse().GetPublicStaticFields().First().GetText().ToString());

    [Fact]
    public void GetConst()
        => Assert.Equal("DateTime variable = null",
            "using System; class Test{public const DateTime variable = null;}".Parse().GetVariables().First().GetText().ToString());

    [Fact]
    public void GetProperties_Simple()
        => Assert.Equal("public DateTime variable { get; set; }",
            "using System; class Test{public DateTime variable { get; set; }}".Parse().GetProperties().First().GetText().ToString());

    [Fact]
    public void GetProperties_Simple2()
        => Assert.Equal("public DateTime variable { get; set; }",
            "using System; class Test{public DateTime variable { get; set; }}".Parse().GetProperties().First().GetText().ToString());

    [Fact]
    public void GetFieldType_System()
        => Assert.Equal("System.DateTime",
            "using System; class Test{public DateTime variable { get; set; }}"
            .Parse(nameof(GetFieldType_System)).GetProperties().First().DescendantNodes()
            .OfType<IdentifierNameSyntax>().First().GetTType().ToString());

    [Fact]
    public void GetFieldType_System_List()
        => Assert.Equal("System.Collections.Generic.List<int>",
            "using System; using System.Collections.Generic; class Test{public List<int> variable { get; set; }}"
            .Parse(nameof(GetFieldType_System_List)).GetProperties().First().DescendantNodes()
            .OfType<GenericNameSyntax>().First().GetTType().ToString());

    [Fact]
    public void GetFieldType_Custom() => Assert.Equal("Test.MyNameSpace.TestType",
                       new[] { "using Test.MyNameSpace; using System; class Test2{public TestType variable { get; set; }}",
                    "using System; namespace Test.MyNameSpace { class TestType{}" }
                       .Parse("a").First().GetProperties().First().DescendantNodes()
                       .OfType<IdentifierNameSyntax>().First().GetTType().ToString());

    [Fact]
    public void GetFieldType_File2()
    {
        var tree = new[] { "using System; namespace Test.MyNameSpace { class Test{public int variable { get; set; }}}",
                    "using Test.MyNameSpace; using System; class Test2{public DateTime variable { get; set; }}" }
                       .Parse(nameof(GetFieldType_File2)).Last();
        SyntaxNodeExtensions.SetSemanticModel(1, nameof(GetFieldType_File2));
        Assert.Equal("System.DateTime",
                       tree.GetProperties().First().DescendantNodes()
                       .OfType<IdentifierNameSyntax>().First().GetTType(nameof(GetFieldType_File2)).ToString());
    }

    [Fact]
    public void GetProperties_Type()
        => Assert.Equal("public DateTime variable { get; set; }",
            "using System; class Test{public DateTime variable { get; set; }".Parse(nameof(GetProperties_Type)).GetProperties().First().GetText().ToString());

}
