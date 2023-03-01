using System;
using System.Collections.Generic;
using CodeAnalyzation.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record MemberAccessExpression(IExpression Expression, IdentifierExpression Name, IType? Type = null) : Expression<MemberAccessExpressionSyntax>(Type ?? TypeShorthands.NullType)  // TODO: Type from semantic analysis
{
    public override MemberAccessExpressionSyntax Syntax()
        => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Expression.Syntax(), Token(SyntaxKind.DotToken), Name.Syntax());
    public IdentifierNameSyntax Syntax(string name) => IdentifierName(name);
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        if (Name.Symbol is not null)
        {
            return Name.Symbol switch
            {
                IFieldSymbol field => new LiteralExpression(SemanticReflection.GetField(field).GetValue(Expression.EvaluatePlain(context))),
                IPropertySymbol property => new LiteralExpression(SemanticReflection.GetProperty(property).GetValue(Expression.EvaluatePlain(context))),
                IMethodSymbol method => throw new NotImplementedException(),
                INamespaceSymbol @namespace => new Namespace(@namespace),
                ITypeSymbol type => new TypeFromSymbol(type),
                _ when Name.Type is not null => Name.Type,
                _ => throw new NotImplementedException($"Evaluate not implemented for MemberAccessExpression '{ToString()}'.")
            };
        }
        var foundType = Type.ReflectedType;
        if (foundType is not null)
        {
            try
            {
                var propertyInfo = foundType.GetProperty(Name.Name);
                return new LiteralExpression(propertyInfo.GetValue(Expression.LiteralValue));
            }
            catch (Exception)
            {
                try
                {
                    var fieldInfo = foundType.GetField(Name.Name);
                    return new LiteralExpression(fieldInfo.GetValue(Expression.LiteralValue));
                }
                catch (Exception)
                {
                }
            }
        }
        throw new NotImplementedException();
    }

    public override string ToString() => $"(Expression: {Expression}, Name: {Name}, Type: {Type})";
}
