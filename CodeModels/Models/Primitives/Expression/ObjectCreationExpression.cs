using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.Execution.Context;
using CodeModels.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record ObjectCreationExpression(IType Type, PropertyCollection? Arguments, InitializerExpression? Initializer, Microsoft.CodeAnalysis.IOperation? Operation = null) : Expression<ObjectCreationExpressionSyntax>(Type, Operation?.Type)
{
    public override ObjectCreationExpressionSyntax Syntax() => ObjectCreationExpression(Type.Syntax(), Arguments?.ToArguments(), Initializer?.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        IExpression? value;
        object? valuePlain = null;
        if (Operation is IObjectCreationOperation objectCreationOperation && SymbolUtils.IsNewDefined(objectCreationOperation))
        {
            // TODO: Remove static reference
            var member = context.ProgramContext.Get<ClassDeclaration>(objectCreationOperation.Type);
            //var memberw = ProgramContext.Context.Get<ClassDeclaration>(objectCreationOperation.Type);
            var constructor = member.GetConstructor();
            var arguments = Arguments is null ? Array.Empty<IExpression>()
               : Arguments.ToExpressions().ToArray();
            var invocation = ConstructorInvocation(constructor, arguments);
            value = invocation.Evaluate(context);
            //return CodeModelFactory.ConstructorInvocation(constructor);
        }
        else
        {
            var constructor = GetConstructor();
            value = Value(constructor.Invoke(Arguments is null ? Array.Empty<IExpression>()
               : Arguments.ToExpressions().Select(x => x.EvaluatePlain(context)).ToArray()));
            valuePlain = value.LiteralValue();
        }
        if (Initializer is not null)
        {
            context.EnterScope(value);
            var initialValues = Initializer.EvaluatePlain(context);
            var isAssignmentInInitializer = Initializer.Expressions.Properties.FirstOrDefault()?.ToExpression() is AssignmentExpression;
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
                        if (addMethod is null) throw new NotImplementedException($"Unhandled ienumerable: '{value}'.");
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

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        throw new NotImplementedException();
    }
}
