using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace CodeAnalyzation.Models
{
    public class EnumModel : MethodHolder
    {
        public ExpressionCollection Values { get; set; }
        public bool IsFlags { get; set; }
        public bool HasNoneValue { get; set; }
        public IEnumerable<IEnumerable<string>>? ValueCategories { get; set; }

        public EnumModel(string identifier, IEnumerable<string>? values = null, Namespace? @namespace = null, bool isFlags = false, bool hasNoneValue = false)
            : base(identifier, properties:new PropertyCollection(values.Select(x =>  Property.FromName(x))), @namespace: @namespace)
        {
            HasNoneValue = hasNoneValue;
            IsFlags = isFlags;
            if (HasNoneValue) values = new string[] { "None" }.Concat(values);
            Values = new ExpressionCollection(values.Select(x => new LiteralExpression(x)));
            IsStatic = true;
        }

        public EnumModel(string identifier, IEnumerable<IEnumerable<string>>? valueCategories = null, Namespace? @namespace = null, bool isFlags = false, bool hasNoneValue = false)
            : this(identifier, valueCategories.SelectMany(x => x), @namespace, isFlags, hasNoneValue)
        {
            ValueCategories = valueCategories;
        }

        public EnumDeclarationSyntax ToEnum() => Values.ToEnum(Name, IsFlags, HasNoneValue);
    }
}