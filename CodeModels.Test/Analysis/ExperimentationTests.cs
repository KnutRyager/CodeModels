#pragma warning disable CS0219 // Variable is assigned but its value is never used

namespace CodeModels.Analysis.Test;

public class ExperimentationTests
{
    public class A { };
    public class B : A { };
    public class C : A { };

    public string T = @"
namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public partial class IdentifierNameSyntax
    {
        internal override string ErrorDisplayName()
        {
            return Identifier.ValueText;
        }
    }
}
";

    //[Fact]
    //public void TestGetClasses()
    //{
    //    var compilation = T.Parse();
    //    var pb = compilation.GetVisit(new PublicOnlyRewriter());
    //    var classes = compilation.GetClasses();
    //    var ns = compilation.GetNamespaces();
    //    var y = 0;
    //}

    //[Fact]
    //public void TryGetDerivedClasses()
    //{
    //    RoslynUtil.ReadRoslyn();
    //    CodeEqual("System.List",
    //                   "using System; class A{} class B : A{} class C : A{}".Parse().GetProperties().FirstOrDefault().DescendantNodes()
    //                   .OfType<IdentifierNameSyntax>().FirstOrDefault().GetTType());
    //}
}
