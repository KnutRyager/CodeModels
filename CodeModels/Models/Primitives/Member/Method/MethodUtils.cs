using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CodeModels.Factory;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Invocation;
using CodeModels.Models.Reflection;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member;

public static class MethodUtils
{
    public static Block? GetBodyFromPreference(IStatementOrExpression? body, MethodBodyPreference preference) => preference switch
    {
        MethodBodyPreference.None => default,
        MethodBodyPreference.Expression or MethodBodyPreference.Automatic => body as Block,
        MethodBodyPreference.Body => Block(body),
        _ => throw new NotImplementedException($"Not implemented: '{preference}'")
    };

    public static IExpression? GetExpressionBodyFromPreference(IStatementOrExpression? body, MethodBodyPreference preference) => preference switch
    {
        MethodBodyPreference.None or MethodBodyPreference.Body => default,
        MethodBodyPreference.Expression or MethodBodyPreference.Automatic => body as IExpression,
        _ => throw new NotImplementedException($"Not implemented: '{preference}'")
    };
}
