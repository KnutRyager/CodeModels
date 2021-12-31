using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Collectors
{
    public class TupleElementFilter : AbstractFilter<TupleElementSyntax>
    {
        public TupleElementFilter()
        {
        }

        public override bool Check(TupleElementSyntax node)
        {
            return true;
        }
    }

    public class TupleElementVisiter : AbstractVisiterCollector<TupleElementSyntax, TupleElementFilter>
    {
        public TupleElementVisiter(TupleElementFilter? filter = null) : base(filter) { }
        public override SyntaxNode VisitTupleElement(TupleElementSyntax node) => HandleVisit(node);
    }
}