﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Emit;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record SizeOfExpression(IType Type) : Expression<SizeOfExpressionSyntax>(Type)
{
    public override SizeOfExpressionSyntax Syntax() => SizeOfExpression(Type.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    // https://stackoverflow.com/questions/8173239/c-getting-size-of-a-value-type-variable-at-runtime
    public override IExpression Evaluate(ICodeModelExecutionContext context) => Type.GetReflectedType() is Type type ? CodeModelFactory.Literal(GetTypeSize(type)) : throw new NotImplementedException();

    static ConcurrentDictionary<Type, int> _cache = new();

    static int GetTypeSize(Type type)
    {
        return _cache.GetOrAdd(type, _ =>
        {
            var dm = new DynamicMethod("SizeOfType", typeof(int), new Type[0]);
            ILGenerator il = dm.GetILGenerator();
            il.Emit(OpCodes.Sizeof, type);
            il.Emit(OpCodes.Ret);
            return (int)dm.Invoke(null, null);
        });
    }
}
