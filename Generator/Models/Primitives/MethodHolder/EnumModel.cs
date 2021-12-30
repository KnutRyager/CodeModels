using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Models
{
    public record EnumModel(string Identifier, ExpressionCollection Values, Namespace? Namespace, bool IsFlags, bool HasNoneValue)
        : MethodHolder(Identifier, new PropertyCollection(Values.Values.Select(x => Property.FromName((x.LiteralValue as string)!))), new(), Namespace, IsStatic: true)
    {
        public IEnumerable<IEnumerable<string>>? ValueCategories { get; set; }

        public EnumModel(string identifier, IEnumerable<string>? values = null, Namespace? @namespace = null, bool isFlags = false, bool hasNoneValue = false)
            : this(identifier, new ExpressionCollection(AddNoneValue(values ?? new List<string>(), hasNoneValue).Select(x => new LiteralExpression(x))), @namespace, isFlags, hasNoneValue) { }

        public EnumModel(string identifier, IEnumerable<IEnumerable<string>>? valueCategories = null, Namespace? @namespace = null, bool isFlags = false, bool hasNoneValue = false)
            : this(identifier, valueCategories.SelectMany(x => x), @namespace, isFlags, hasNoneValue)
        {
            ValueCategories = valueCategories;
        }

        public EnumDeclarationSyntax ToEnum() => Values.ToEnum(Name, IsFlags, HasNoneValue);

        private static List<string> AddNoneValue(IEnumerable<string> values, bool hasNoneValue)
        {
            if (hasNoneValue && !values.Contains("None")) values = new string[] { "None" }.Concat(values);
            return values.ToList();
        }
    }
}