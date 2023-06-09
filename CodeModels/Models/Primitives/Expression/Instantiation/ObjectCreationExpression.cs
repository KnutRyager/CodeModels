﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using CodeModels.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Expression.Instantiation;

public record ObjectCreationExpression(IType Type, ArgumentList? Arguments, InitializerExpression? Initializer, Microsoft.CodeAnalysis.IOperation? Operation = null)
    : Expression<ObjectCreationExpressionSyntax>(Type, Operation?.Type)
{
    public static ObjectCreationExpression Create(IType type,
        IEnumerable<IToArgumentConvertible>? arguments = null,
        InitializerExpression? initializer = null,
        Microsoft.CodeAnalysis.IOperation? operation = null)
        => new(type, arguments is null ? null : ArgList(arguments), initializer, operation);

    public override ObjectCreationExpressionSyntax Syntax() => ObjectCreationExpression(Type.Syntax(),
        Arguments?.Syntax(), Initializer?.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Arguments is not null) yield return Arguments;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        IExpression? value;
        object? valuePlain = null;
        if (Operation is IObjectCreationOperation objectCreationOperation && SymbolUtils.IsNewDefined(objectCreationOperation))
        {
            var member = context.ProgramContext.Get<IClassDeclaration>(objectCreationOperation.Type ?? throw new NotImplementedException());
            var constructor = member.GetConstructor();
            var arguments = Arguments?.Arguments.Select(x => x.Expression).ToArray() ?? Array.Empty<IExpression>();
            var invocation = ConstructorInvocation(constructor, arguments);
            value = invocation.Evaluate(context);
        }
        else
        {
            var constructor = GetConstructor();
            value = Value(constructor.Invoke(Arguments?.EvaluatePlain(context).ToArray() ?? Array.Empty<object>()));
            valuePlain = value.LiteralValue();
        }
        if (Initializer is not null)
        {
            context.EnterScope(value);
            var initialValues = Initializer.EvaluatePlain(context);
            var isAssignmentInInitializer = Initializer.Expressions.FirstOrDefault() is AssignmentExpression;
            if (!isAssignmentInInitializer)
            {
                if (initialValues is IEnumerable<object?> initialPlainValues)
                {
                    if (valuePlain is System.Collections.IDictionary dictionary)
                    {
                        foreach (var v in initialPlainValues)
                        {
                            if (v is object[] array) dictionary[array[0]] = array[1];
                        }
                    }
                    else if (valuePlain is System.Collections.IEnumerable collection)
                    {
                        var addMethod = collection.GetType().GetMethod("Add");
                        if (addMethod is null) throw new NotImplementedException($"Unhandled IEnumerable: '{value}'.");
                        foreach (var v in initialPlainValues) addMethod.Invoke(collection, new[] { v });
                    }
                }
            }
            context.ExitScope(value);
        }
        return value ?? VoidValue;
    }

    private ConstructorInfo GetConstructor()
    {
        if (Operation is IObjectCreationOperation objectCreationOperation)
        {
            return SemanticReflection.GetConstructor(objectCreationOperation);
        }
        else
        {
            var type = Type.ReflectedType;
            var parameters = Arguments?.Arguments.Select(x => x.Expression.Get_Type().ReflectedType).ToArray();
            var constructor = type?.GetConstructor(parameters);
            if (constructor is not null)
            {
                return constructor;
            }
        }
        throw new NotImplementedException();
    }
}