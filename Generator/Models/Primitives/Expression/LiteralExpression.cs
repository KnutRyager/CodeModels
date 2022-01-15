using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Newtonsoft.Json;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;


namespace CodeAnalyzation.Models
{
    public record LiteralExpression(IType Type, object? Value, string? SerializedValue) : Expression<LiteralExpressionSyntax>(Type)
    {
        public LiteralExpression(IType type, object? value = null) : this(type, value, "missing_json_serializer") { }
        //public LiteralExpression(IType type, object? value = null) : this(type, value, JsonConvert.SerializeObject(value)) { }

        public LiteralExpression(object value) : this(new TypeFromReflection(value.GetType()), value) { }

        public LiteralExpression(AbstractType type, string serializedValue) : this(type, null, serializedValue)
        {
            var reflectedType = Type.GetReflectedType();
            //if (reflectedType != null)
            //    Value = JsonConvert.DeserializeObject(SerializedValue!, reflectedType);
        }

        public LiteralExpression(EnumMemberDeclarationSyntax value) : this(new TypeFromReflection(typeof(string)), value.Identifier) { }
        public override LiteralExpressionSyntax LiteralSyntax => LiteralExpressionCustom(Value) ;
        public override object? LiteralValue => Value;
        public override LiteralExpressionSyntax Syntax() => LiteralSyntax;
    }
}