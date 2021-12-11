using System.Collections.Generic;

namespace CodeAnalyzation.Models.Syntax
{
    public abstract class MSyntaxNode
    {
        public List<MSyntaxNode> Descendants { get; set; } = new List<MSyntaxNode>();

        //public abstract SyntaxNode Kind { get; }
    }
}