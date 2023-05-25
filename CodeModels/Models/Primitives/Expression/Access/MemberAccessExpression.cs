using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Factory;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using CodeModels.Models.Primitives.Member;
using CodeModels.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Expression.Access;

public record MemberAccessExpression(IExpression Expression, IdentifierExpression Identifier, IType? Type = null)
    : Expression<MemberAccessExpressionSyntax>(Type ?? TypeShorthands.NullType),
    IAssignable, IScopeHolder
// TODO: Type from semantic analysis
{
    public override MemberAccessExpressionSyntax Syntax()
        => MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Expression.Syntax(), Token(SyntaxKind.DotToken), Identifier.Syntax());
    public IdentifierNameSyntax Syntax(string name) => IdentifierName(name);
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Expression;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        var expression = Expression.Evaluate(context);
        var instance = expression as IInstantiatedObject;
        //var instance = expression as InstantiatedObject ?? (Expression is IdentifierExpression i ? i.Model as InstantiatedObject : null);
        var lookup = Identifier.Lookup(context);
        var declaration = instance?.Type ?? lookup as IMember;
        if (declaration is IClassDeclaration classDeclaration)
        {
            var access = classDeclaration.GetMember(Identifier.Name);
            if (access is IInvokable invokable)
            {
                return invokable.Invoke(expression, Array.Empty<IExpression>());
            }

            //var constructor = member.GetConstructor();
            //return CodeModelFactory.ConstructorInvocation(constructor);
        }
        if (lookup is not null)
        {
            return lookup switch
            {
                IInvokable invokable => invokable.Invoke(instance, Array.Empty<IExpression>()),
                Namespace @namespace => @namespace,
                IType type => type,
                _ when Identifier.Type is not null => Identifier.Type,
                _ => throw new NotImplementedException($"Evaluate not implemented for MemberAccessExpression '{ToString()}'.")
            };
        }
        else if (Identifier.Symbol is not null)
        {
            if (SymbolUtils.IsNewDefined(Identifier.Symbol))
            {
                //var declaration = ProgramContext.Context.Get<ICodeModel>(Identifier.Symbol) as IMember;
                if (Identifier.Symbol.IsStatic)
                {
                    if (Identifier.Symbol is IFieldSymbol)
                    {
                        return ((declaration as IField).Owner as IClassDeclaration).GetStaticScope().GetValue(Identifier.Name);
                    }
                }
                return Identifier.Symbol switch
                {
                    IFieldSymbol field => throw new NotImplementedException(),
                    IPropertySymbol property => CodeModelFactory.Literal(SemanticReflection.GetProperty(property).GetValue(Expression.EvaluatePlain(context))),
                    IMethodSymbol method => throw new NotImplementedException(),
                    INamespaceSymbol @namespace => new Namespace(@namespace),
                    ITypeSymbol type => TypeFromSymbol.Create(type),
                    _ when Identifier.Type is not null => Identifier.Type,
                    _ => throw new NotImplementedException($"Evaluate not implemented for MemberAccessExpression '{ToString()}'.")
                };
            }
            else
            {
                return Identifier.Symbol switch
                {
                    IFieldSymbol field => CodeModelFactory.Literal(SemanticReflection.GetField(field).GetValue(Expression.EvaluatePlain(context))),
                    IPropertySymbol property => CodeModelFactory.Literal(SemanticReflection.GetProperty(property).GetValue(Expression.EvaluatePlain(context))),
                    IMethodSymbol method => throw new NotImplementedException(),
                    INamespaceSymbol @namespace => new Namespace(@namespace),
                    ITypeSymbol type => TypeFromSymbol.Create(type),
                    _ when Identifier.Type is not null => Identifier.Type,
                    _ => throw new NotImplementedException($"Evaluate not implemented for MemberAccessExpression '{ToString()}'.")
                };
            }
        }
        var foundType = Type.ReflectedType;
        if (foundType is not null)
        {
            try
            {
                var propertyInfo = foundType.GetProperty(Identifier.Name);
                return CodeModelFactory.Literal(propertyInfo.GetValue(Expression.LiteralValue()));
            }
            catch (Exception)
            {
                try
                {
                    var fieldInfo = foundType.GetField(Identifier.Name);
                    return CodeModelFactory.Literal(fieldInfo.GetValue(Expression.LiteralValue()));
                }
                catch (Exception)
                {
                }
            }
        }
        return CodeModelFactory.NullValue;
    }

    public override string ToString() => $"(Expression: {Expression}, Name: {Name}, Type: {Type})";

    public void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes)
    {
        if (scopes.Count is 0) scopes = GetScopes(context);
        try
        {
            context.EnterScopes(scopes);
            context.SetValue(Identifier, value);
        }
        finally
        {
            context.ExitScopes(scopes);
        }
    }

    public IList<ICodeModelExecutionScope> GetScopes(ICodeModelExecutionContext context)
    {
        var expression = Expression.Evaluate(context);
        if (expression is IScopeHolder scopeHolder) return scopeHolder.GetScopes(context);
        var member = Identifier.Lookup(context);
        if (member is IScopeHolder memberScopeHolder)
        {
            return memberScopeHolder.GetScopes(context);
        }
        throw new NotImplementedException();
    }
}
