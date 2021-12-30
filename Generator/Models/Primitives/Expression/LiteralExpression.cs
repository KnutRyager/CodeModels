using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;


namespace CodeAnalyzation.Models
{
    public record LiteralExpression(TType Type, object? Value, string? SerializedValue) : Expression(Type)
    {
        public LiteralExpression(TType type, object? value = null) : this(type, value, JsonConvert.SerializeObject(value)) { }

        public LiteralExpression(object value) : this(new(value.GetType()), value) { }

        public LiteralExpression(TType type, string serializedValue) : this(type, null, serializedValue)
        {
            var reflectedType = Type.GetReflectedType();
            if (reflectedType != null)
                Value = JsonConvert.DeserializeObject(SerializedValue!, reflectedType);
        }

        public LiteralExpression(EnumMemberDeclarationSyntax value) : this(new TType(typeof(string)), value.Identifier) { }
        public override LiteralExpressionSyntax? LiteralSyntax => Value != null ? LiteralExpressionCustom(Value) : base.LiteralSyntax;
        public override object? LiterallyValue => Value;

    }
}