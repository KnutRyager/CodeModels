using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace CodeAnalyzation.Models
{
    public class EnumModel : MethodHolder
    {
        public ValueCollection Values { get; set; }
        public bool IsFlags { get; set; }
        public bool HasNoneValue { get; set; }

        public EnumModel(string identifier, IEnumerable<string>? values = null, Namespace? @namespace = null, bool isFlags = false, bool hasNoneValue = false)
            : base(identifier, @namespace: @namespace)
        {
            HasNoneValue = hasNoneValue;
            IsFlags = isFlags;
            if (HasNoneValue) values = new string[] { "None" }.Concat(values);
            Values = new ValueCollection(values.Select(x => Value.FromValue(x)));
        }

        public EnumDeclarationSyntax ToEnum() => Values.ToEnum(Name, IsFlags, HasNoneValue);
    }
}