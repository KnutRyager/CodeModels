using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record VariableDeclarator(string Name, IExpression? Value)
    : CodeModel<VariableDeclaratorSyntax>
{
    public static VariableDeclarator Create(string name, IExpression? value = null)
        => new(name, value);

    public override VariableDeclaratorSyntax Syntax() => VariableDeclaratorCustom(Identifier(Name), Value?.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        if (Value is not null) yield return Value;
    }

    public void Evaluate(ICodeModelExecutionContext context)
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
