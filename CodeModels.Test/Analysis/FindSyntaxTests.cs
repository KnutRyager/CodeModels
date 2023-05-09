using System.Linq;
using CodeAnalyzation.Parsing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace CodeAnalyzation.Analysis.Test;

public class FindSyntaxTests
{
    [Fact]
    public void GetPublicStaticFields()
        => Assert.Equal("public const DateTime variable = null;",
            "using System; class Test{public const DateTime variable = null;}".Parse(nameof(GetPublicStaticFields)).GetPublicStaticFields().First().GetText().ToString());

    [Fact]
    public void GetConst()
        => Assert.Equal("DateTime variable = null",
            "using System; class Test{public const DateTime variable = null;}".Parse(nameof(GetConst)).GetVariables().First().GetText().ToString());

    [Fact]
    public void GetProperties_Simple()
        => Assert.Equal("public DateTime variable { get; set; }",
            "using System; class Test{public DateTime variable { get; set; }}".Parse(nameof(GetProperties_Simple)).GetProperties().First().GetText().ToString());

    [Fact]
    public void GetProperties_Simple2()
        => Assert.Equal("public DateTime variable { get; set; }",
            "using System; class Test{public DateTime variable { get; set; }}".Parse(nameof(GetProperties_Simple2)).GetProperties().First().GetText().ToString());

    [Fact]
    public void GetFieldType_System()
        => Assert.Equal("System.DateTime",
            "using System; class Test{public DateTime variable { get; set; }}"
            .Parse(nameof(GetFieldType_System)).GetProperties().First().DescendantNodes()
            .OfType<IdentifierNameSyntax>().First().GetType(nameof(GetFieldType_System)).ToString());

    [Fact]
    public void GetFieldType_System_List()
        => Assert.Equal("System.Collections.Generic.List<int>",
            "using System; using System.Collections.Generic; class Test{public List<int> variable { get; set; }}"
            .Parse(nameof(GetFieldType_System_List)).GetProperties().First().DescendantNodes()
            .OfType<GenericNameSyntax>().First().GetType(nameof(GetFieldType_System_List)).ToString());

    [Fact]
    public void GetFieldType_Custom() => Assert.Equal("Test.MyNameSpace.TestType",
                       (new[] { "using Test.MyNameSpace; using System; class Test2{public TestType variable { get; set; }}",
                    "using System; namespace Test.MyNameSpace { class TestType{}" })
                       .Parse(nameof(GetFieldType_Custom)).First().GetProperties().First().DescendantNodes()
                       .OfType<IdentifierNameSyntax>().First().GetType(nameof(GetFieldType_Custom)).ToString());

    [Fact]
    public void GetFieldType_File2() => Assert.Equal("System.DateTime",
                       new[] { "using System; namespace Test.MyNameSpace { class Test{public int variable { get; set; }}}",
                    "using Test.MyNameSpace; using System; class Test2{public DateTime variable { get; set; }}" }
                       .Parse(nameof(GetFieldType_File2)).Last().GetProperties().First().DescendantNodes()
                       .OfType<IdentifierNameSyntax>().First().GetType(nameof(GetFieldType_File2), 1).ToString());

    [Fact]
    public void GetProperties_Type()
        => Assert.Equal("public DateTime variable { get; set; }",
            "using System; class Test{public DateTime variable { get; set; }".Parse(nameof(GetProperties_Type)).GetProperties().First().GetText().ToString());

}
