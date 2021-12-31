using Xunit;

namespace CodeAnalyzation.Parsing.Test;

[Collection("Sequential")]
public class ParseTests
{
    [Fact]
    public void EmptyNamespace()
        => Assert.Equal("[]", "namespace T{}".ParseToJson().SingleQuote());

    //[Fact]
    //public void SimpleClass()
    //    => Assert.Equal("[{'Identifier':'Test','Namespace':null,'Fields':[],'Methods':[],'Constants':[]}]",
    //        "public class Test{}".ParseToJson().SingleQuote());

    //[Fact]
    //public void Parse_Class()
    //    => Assert.Equal("[{'Identifier':'Test','Namespace':{'Parts':['T']},'Fields':[],'Methods':[],'Constants':[]}]",
    //        "namespace T{ public class Test{}}".ParseToJson().SingleQuote());

    //[Fact]
    //public void Parse_Type()
    //    => Assert.Equal("[{'Identifier':'Test','Namespace':null,'Methods':[],'Constants':[]}]",
    //        "using System; static class Test{public const DateTime variable;}".ParseToJson().SingleQuote());

    //[Fact]
    //public void Parse_ClassWithProperties()
    //    => Assert.Equal("[{'Identifier':'Test','Namespace':{'Parts':['T']},'Fields':[],'Methods':[],'Constants':[]}]",
    //        "namespace T{ public class Test{public int A{get;set;}}}".ParseToJson().SingleQuote());

}
