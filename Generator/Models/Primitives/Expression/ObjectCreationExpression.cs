﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeAnalyzation.Models.CodeModelFactory;
using Microsoft.CodeAnalysis.Operations;
using System.Reflection;

namespace CodeAnalyzation.Models;

public record ObjectCreationExpression(IType Type, PropertyCollection? Arguments, InitializerExpression? Initializer, Microsoft.CodeAnalysis.IOperation? Operation = null) : Expression<ObjectCreationExpressionSyntax>(Type)
{
    public override ObjectCreationExpressionSyntax Syntax() => ObjectCreationExpression(Type.Syntax(), Arguments?.ToArguments(), Initializer?.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        var constructor = GetConstructor();
        var value = constructor.Invoke(Arguments is null ? Array.Empty<IExpression>()
            : Arguments.ToExpressions().Select(x => x.EvaluatePlain(context)).ToArray());
        if (Initializer is not null)
        {
            context.EnterScope(value);
            var initialValues = Initializer.EvaluatePlain(context);
            var isAssignmentInInitializer = Initializer.Expressions.Properties.FirstOrDefault()?.ToExpression() is AssignmentExpression;
            if (!isAssignmentInInitializer)
            {
                if (initialValues is IEnumerable<object?> initialPlainValues)
                {
                    if (value is System.Collections.IDictionary dictionary)
                    {
                        foreach (var v in initialPlainValues)
                        {
                            if (v is object[] array) dictionary[array[0]] = array[1];
                        }
                    }
                    else if (value is System.Collections.IEnumerable collection)
                    {
                        var addMethod = collection.GetType().GetMethod("Add");
                        if (addMethod is null) throw new NotImplementedException($"Unhandled ienumerable: '{value}'.");
                        foreach (var v in initialPlainValues) addMethod.Invoke(collection, new[] { v });
                    }
                }
            }
            context.ExitScope(value);
        }
        return Value(value);
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
            var parameters = Arguments?.Properties.Select(x => x.Get_Type().ReflectedType).ToArray();
            var constructor = type.GetConstructor(parameters);
            if (constructor is not null)
            {
                return constructor;
            }
        }
        throw new NotImplementedException();
    }
}

public record ImplicitObjectCreationExpression(IType Type, PropertyCollection Arguments, InitializerExpression? Initializer) : Expression<ImplicitObjectCreationExpressionSyntax>(Type)
{
    public override ImplicitObjectCreationExpressionSyntax Syntax() => ImplicitObjectCreationExpression(Arguments.ToArguments(), Initializer?.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        throw new NotImplementedException();
    }
}
