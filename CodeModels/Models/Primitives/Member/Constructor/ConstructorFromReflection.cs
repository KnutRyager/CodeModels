﻿using System.Collections.Generic;
using System.Reflection;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Invocation;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models.Primitives.Member;

public record ConstructorFromReflection(ConstructorInfo Constructor)
    : MethodBase<ConstructorDeclarationSyntax, ConstructorInvocationExpression>(
        TypeFromReflection.Create(Constructor),
        Constructor.Name,
        CodeModelsFromReflection.ParamList(Constructor),
        CodeModelFactory.AttributesList(),   // TODO
        Modifier.Public), IConstructor
{
    public override IEnumerable<ICodeModel> Children()
    {
        throw new System.NotImplementedException();
    }

    public override ConstructorInvocationExpression Invoke(IExpression? caller, IEnumerable<IExpression> arguments)
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
