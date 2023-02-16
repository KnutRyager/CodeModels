using System.Collections.Generic;
using CodeAnalyzation.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record IdentifierExpression(string Name, IType? Type = null, ISymbol? Symbol = null)
    : Expression<IdentifierNameSyntax>(Type ?? (Symbol is ITypeSymbol typeSymbol ? new TypeFromSymbol(typeSymbol) : TypeShorthands.VoidType), Symbol)
{
    public IdentifierExpression(ISymbol symbol) : this(symbol.Name, null, symbol) { }
    public override IdentifierNameSyntax Syntax() => Syntax(Name ?? Type.Name);
    public IdentifierNameSyntax Syntax(string name) => IdentifierName(name);
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context) => context.GetValue(this);
    public override object? EvaluatePlain(IProgramModelExecutionContext context) => Evaluate(context).EvaluatePlain(context);
    public override IdentifierExpression GetIdentifier() => this;

    public System.Type? GetReflectedType() => Type?.GetReflectedType() ?? (Symbol is ITypeSymbol typeSymbol ? SemanticReflection.GetType(typeSymbol) : null);

    public override string ToString() => $"IdentifierExpression(Name: {Name}, Type: {Type}, Symbol: {Symbol})";
}
