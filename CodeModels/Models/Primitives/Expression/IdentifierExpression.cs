using System.Collections.Generic;
using System.ComponentModel;
using CodeModels.Models.ProgramModels;
using CodeModels.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record IdentifierExpression(string Name, IType? Type = null, ISymbol? Symbol = null, ICodeModel? Model = null)
    : Expression<IdentifierNameSyntax>(Type ?? (Symbol is ITypeSymbol typeSymbol ? new TypeFromSymbol(typeSymbol) : TypeShorthands.VoidType), Symbol, Name),
    IAssignable
{
    public IdentifierExpression(ISymbol symbol) : this(symbol.Name, null, symbol) { }
    public override IdentifierNameSyntax Syntax() => Syntax(Name ?? Type.Name);
    public SyntaxToken ToToken() => SyntaxFactory.Identifier(Name);
    public IdentifierNameSyntax Syntax(string name) => IdentifierName(name);
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context) => context.GetValue(this);

    public ICodeModel? Lookup(IProgramModelExecutionContext context)
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

    private IMember? ResolveSymbol(IProgramModelExecutionContext context)
    {
        if (Symbol is not null && SymbolUtils.IsNewDefined(Symbol))
        {
            return context.ProgramContext.Get<IMember>(Symbol);
        }
        return null;
    }

    public override object? EvaluatePlain(IProgramModelExecutionContext context) => Evaluate(context).EvaluatePlain(context);
    public override IdentifierExpression ToIdentifierExpression() => this;

    public System.Type? GetReflectedType() => Type?.GetReflectedType() ?? (Symbol is ITypeSymbol typeSymbol ? SemanticReflection.GetType(typeSymbol) : null);

    public AssignmentExpression Assign(IExpression value) => CodeModelFactory.Assignment(this, value);

    public override string ToString() => $"IdentifierExpression(Name: {Name}, Type: {Type}, Symbol: {Symbol})";

    public void Assign(IExpression value, IProgramModelExecutionContext context, IList<IProgramModelExecutionScope> scopes)
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
}
