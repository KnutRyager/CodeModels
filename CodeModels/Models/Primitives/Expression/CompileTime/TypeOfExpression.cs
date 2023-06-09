﻿using System;
using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Expression.CompileTime;

public record TypeOfExpression(IType Type) : Expression<TypeOfExpressionSyntax>(Type)
{
    public static TypeOfExpression Create(IType type) => new(type);

    public override TypeOfExpressionSyntax Syntax() => TypeOfExpression(Type.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context) => Type.GetReflectedType() is Type type ? CodeModelFactory.Literal(type) : throw new NotImplementedException();
}
