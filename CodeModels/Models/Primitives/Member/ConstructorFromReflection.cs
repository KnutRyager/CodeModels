using System.Collections.Generic;
using System.Reflection;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Invocation;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models;

public record ConstructorFromReflection(ConstructorInfo Constructor)
    : MethodBase<ConstructorDeclarationSyntax, ConstructorInvocationExpression>(
        new TypeFromReflection(Constructor.DeclaringType),
        Constructor.Name,
        null,   // TODO
        Modifier.Public), IConstructor
{
    public override IEnumerable<ICodeModel> Children()
    {
        throw new System.NotImplementedException();
    }

    public override ConstructorInvocationExpression Invoke(IExpression caller, IEnumerable<IExpression> arguments)
    {
        throw new System.NotImplementedException();
    }

    public override CodeModel<ConstructorDeclarationSyntax> Render(Namespace @namespace)
    {
        throw new System.NotImplementedException();
    }

    public override ConstructorDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
    {
        throw new System.NotImplementedException();
    }
}
