using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Factory;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using CodeModels.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Models.Primitives.Expression.Reference;

public record IdentifierExpression(string Name, IType? Type = null, ISymbol? Symbol = null, ICodeModel? Model = null)
    : Expression<IdentifierNameSyntax>(Type ?? (Symbol is ITypeSymbol typeSymbol ? TypeFromSymbol.Create(typeSymbol) : TypeShorthands.VoidType), Symbol, Name),
    IAssignable, IToParameterConvertible
{
    public static IdentifierExpression Create(string name, IType? type = null, ISymbol? symbol = null, ICodeModel? model = null)
        => new(name, type, symbol, model);
    public static IdentifierExpression Create(ISymbol symbol) => new(symbol.Name, null, symbol);

    public override IdentifierNameSyntax Syntax() => Syntax(Name ?? Type.Name);
    public SyntaxToken ToToken() => Identifier(Name);
    public IdentifierNameSyntax Syntax(string name) => IdentifierName(name);
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context) => context.GetValue(this);

    public ICodeModel? Lookup(ICodeModelExecutionContext context)
    {
        if (Model is not null) return Model;
        if (Symbol is not null)
        {
            try
            {
                return ResolveSymbol(context);
            }
            catch
            { }
        }
        return context.TryGetValueOrMember(this);
    }

    private IMember? ResolveSymbol(ICodeModelExecutionContext context)
    {
        if (Symbol is not null && SymbolUtils.IsNewDefined(Symbol))
        {
            return context.ProgramContext.Get<IMember>(Symbol);
        }
        return null;
    }

    public override object? EvaluatePlain(ICodeModelExecutionContext context) => Evaluate(context).EvaluatePlain(context);
    public override IdentifierExpression ToIdentifierExpression() => this;

    public System.Type? GetReflectedType() => Type?.GetReflectedType() ?? (Symbol is ITypeSymbol typeSymbol ? SemanticReflection.GetType(typeSymbol) : null);

    public AssignmentExpression Assign(IExpression value) => CodeModelFactory.Assignment(this, value);

    public override string ToString() => $"IdentifierExpression(Name: {Name}, Type: {Type}, Symbol: {Symbol})";

    public void Assign(IExpression value, ICodeModelExecutionContext context, IList<ICodeModelExecutionScope> scopes)
    {
        try
        {
            //var lookup = Lookup(context);
            context.EnterScopes(scopes);
            context.SetValue(Name, value.Evaluate(context));
            return;
            //if (lookup is IAssignable assignable)
            //{
            //    assignable.Assign(value, context, scopes);
            //    return;
            //}
        }
        finally
        {
            context.ExitScopes(scopes);
        }

        throw new System.NotImplementedException();
    }

    public Parameter ToParameter() => Param(Name, Type);
}
