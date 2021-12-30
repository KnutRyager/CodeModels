using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;


namespace CodeAnalyzation.Models
{
    public class LiteralExpression : Expression
    {
        public object? Value { get; set; }
        public string? SerializedValue { get; set; }

        public LiteralExpression(TType type, object? value = null) : base(type)
        {
            Value = value;
            SerializedValue = JsonConvert.SerializeObject(value);
        }

        public LiteralExpression(object value) : this(new(value.GetType()), value) { }

        public LiteralExpression(TType type, string serializedValue) : base(type)
        {
            SerializedValue = serializedValue;
            var reflectedType = Type.GetReflectedType();
            if (reflectedType != null)
                Value = JsonConvert.DeserializeObject(SerializedValue!, reflectedType);
        }

        public LiteralExpression(EnumMemberDeclarationSyntax value) : this(new TType(typeof(string)), value.Identifier) { }

        public override LiteralExpressionSyntax? LiteralSyntax => Value != null ? LiteralExpressionCustom(Value) : base.LiteralSyntax;

        public override object? LiterallyValue => Value;
        public override string ToString() => $"LiteralValue: '{Value}'";

    }
}