using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record VariableDeclaration(IType Type, string Name, IExpression? Value)
    : CodeModel<VariableDeclarationSyntax>, ITypeModel
{
    public static VariableDeclaration Create(IType type, string name, IExpression? value = null)
        => new(type, name, value);

    public IType Get_Type() => Type;
    public override VariableDeclarationSyntax Syntax() => VariableDeclarationCustom(Type.Syntax()!, VariableDeclaratorCustom(Identifier(Name), Value?.Syntax()));
    public TypeSyntax TypeSyntax() => Type.Syntax();
    public VariableDeclarator ToDeclarator() => new(Name, Value);
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Value is not null) yield return Value;
    }

    public void Evaluate(ICodeModelExecutionContext context)
    {
        context.DefineVariable(Name);
        if (Value is not null)
        {
            context.SetValue(Name, Value);
        }
    }
}