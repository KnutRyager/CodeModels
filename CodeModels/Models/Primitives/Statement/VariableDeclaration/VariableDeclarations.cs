using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.Models;

public record VariableDeclarations(IType Type, List<VariableDeclarator> Value)
    : CodeModel<VariableDeclarationSyntax>, ITypeModel
{
    public static VariableDeclarations Create(IType type, IEnumerable<VariableDeclarator>? value = null)
        => new(type, CodeModelFactory.List(value));
    public static VariableDeclarations Create(VariableDeclaration? declaration)
        => Create(declaration?.Type ?? TypeShorthands.VoidType,
            declaration is null ? new List<VariableDeclarator>() :
            CodeModelFactory.List(declaration.ToDeclarator()));
    public static VariableDeclarations Create(IType Type, IEnumerable<(string Name, IExpression? Value)> initializations)
        => Create(Type, initializations.Select(x => new VariableDeclarator(x.Name, x.Value)));
    public static VariableDeclarations Create(IType Type, string name, IExpression? value = null)
        => Create(Type, value is null ? new List<VariableDeclarator>() : CodeModelFactory.List(new VariableDeclarator(name, value)));

    public VariableDeclaration First() => CodeModelFactory.VariableDeclaration(Type, Value.First().Name, Value.First().Value);
    public IType Get_Type() => Type;
    public override VariableDeclarationSyntax Syntax() => VariableDeclarationCustom(Type.Syntax()!, Value.Select(x => x.Syntax()));
    public TypeSyntax TypeSyntax() => Type.Syntax();
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        foreach (var value in Value) yield return value;
    }

    public void Evaluate(ICodeModelExecutionContext context)
    {
        foreach (var declarator in Value)
        {
            declarator.Evaluate(context);
        }
    }
}