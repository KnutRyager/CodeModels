using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Models.CodeModelFactory;

namespace CodeAnalyzation.Models
{
    public record EnumModel(string Identifier, ExpressionCollection Values, Namespace? Namespace, bool IsFlags, bool HasNoneValue)
        : MethodHolder<EnumDeclarationSyntax>(Identifier, new(Values.Values.Select(x => Property((x.LiteralValue as string)!))), new(), Namespace, TopLevelModifier: Modifier.Static)
    {
        public IEnumerable<IEnumerable<string>>? ValueCategories { get; set; }

        public EnumModel(string identifier, IEnumerable<string>? values = null, Namespace? @namespace = null, bool isFlags = false, bool hasNoneValue = false)
            : this(identifier, new ExpressionCollection(Literals(AddNoneValue(values ?? new List<string>(), hasNoneValue))), @namespace, isFlags, hasNoneValue) { }

        public EnumModel(string identifier, IEnumerable<IEnumerable<string>>? valueCategories = null, Namespace? @namespace = null, bool isFlags = false, bool hasNoneValue = false)
            : this(identifier, valueCategories.SelectMany(x => x), @namespace, isFlags, hasNoneValue)
        {
            ValueCategories = valueCategories;
        }

        public EnumDeclarationSyntax ToEnum() => Values.ToEnum(Name, IsFlags, HasNoneValue);
        public override EnumDeclarationSyntax Syntax() => ToEnum();

        private static List<string> AddNoneValue(IEnumerable<string> values, bool hasNoneValue)
        {
            if (hasNoneValue && !values.Contains("None")) values = new string[] { "None" }.Concat(values);
            return values.ToList();
        }
    }

    public record EnumFromReflection(Type Type) : EnumModel(Type.Name, Type.GetFields().Select(x => x.Name), Namespace(Type));
}