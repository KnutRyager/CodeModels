using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record VariableDeclaration(IType Type, string Name, IExpression? Value = null) : CodeModel<VariableDeclarationSyntax>, ITypeModel
{
    public IType Get_Type() => Type;
    public override VariableDeclarationSyntax Syntax() => VariableDeclarationCustom(Type.Syntax()!, VariableDeclaratorCustom(Identifier(Name), Value?.Syntax()));
    public TypeSyntax TypeSyntax() => Type.Syntax();
    public VariableDeclarator ToDeclarator() => new(Name, Value);
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Value is not null) yield return Value;
    }

    public void Evaluate(IProgramModelExecutionContext context)
    {
        context.DefineVariable(Name);
        if (Value is not null)
        {
            context.SetValue(Name, Value);
        }
    }
}

public record VariableDeclarations(IType Type, List<VariableDeclarator> Value) : CodeModel<VariableDeclarationSyntax>, ITypeModel
{
    public VariableDeclarations(VariableDeclaration? declaration) : this(declaration?.Type ?? TypeShorthands.VoidType, declaration is null ? new List<VariableDeclarator>() : CodeModelFactory.List(declaration.ToDeclarator())) { }
    public VariableDeclarations(IType Type, IEnumerable<VariableDeclarator> initializations) : this(Type, CodeModelFactory.List(initializations)) { }
    public VariableDeclarations(IType Type, IEnumerable<(string Name, IExpression? Value)> initializations)
        : this(Type, initializations.Select(x => new VariableDeclarator(x.Name, x.Value))) { }
    public VariableDeclarations(IType Type, string name, IExpression? value = null)
        : this(Type, value is null ? new List<VariableDeclarator>() : CodeModelFactory.List(new VariableDeclarator(name, value))) { }
    public VariableDeclaration First() => new(Type, Value.First().Name, Value.First().Value);
    public IType Get_Type() => Type;
    public override VariableDeclarationSyntax Syntax() => VariableDeclarationCustom(Type.Syntax()!, Value.Select(x => x.Syntax()));
    public TypeSyntax TypeSyntax() => Type.Syntax();
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        foreach (var value in Value) yield return value;
    }

    public void Evaluate(IProgramModelExecutionContext context)
    {
        foreach (var declarator in Value)
        {
            declarator.Evaluate(context);
        }
    }
}

public record VariableDeclarator(string Name, IExpression? Value = null) : CodeModel<VariableDeclaratorSyntax>
{
    public override VariableDeclaratorSyntax Syntax() => VariableDeclaratorCustom(Identifier(Name), Value?.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        if (Value is not null) yield return Value;
    }

    public void Evaluate(IProgramModelExecutionContext context)
    {
        context.DefineVariable(Name);
        if (Value is not null)
        {
            context.SetValue(Name, Value.Evaluate(context));
            // TODO: WHY?
            //context.SetValue(Name, CodeModelFactory.Literal(Value.EvaluatePlain(context)));
        }
    }
}
