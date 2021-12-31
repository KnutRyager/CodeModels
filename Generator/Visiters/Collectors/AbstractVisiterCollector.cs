using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Collectors
{
    public abstract class AbstractFilter<T>
        where T : SyntaxNode
    {
        public abstract bool Check(T node);
    }

    public abstract class AbstractVisiterCollector<T, TFilter> : CSharpSyntaxRewriter
        where T : SyntaxNode
        where TFilter : AbstractFilter<T>, new()
    {
        private readonly List<T> entries = new();
        private readonly TFilter filter;

        public AbstractVisiterCollector(TFilter? filter = null)
        {
            this.filter = filter ?? new TFilter();
        }

        protected SyntaxNode HandleVisit(T node)
        {
            if (filter == null || filter.Check(node))
                entries.Add(node);

            return node;
        }

        public List<T> GetValues(SyntaxTree tree)
        {
            Visit(tree.GetRoot());
            return entries;
        }

        public List<T> GetEntries(IEnumerable<SyntaxTree> trees)
        {
            foreach (var tree in trees) Visit(tree.GetRoot());
            return entries;
        }
    }
}