using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeAnalyzation.Models.CodeModelFactory;
using Microsoft.CodeAnalysis.Operations;

namespace CodeAnalyzation.Models;

// TODO: Determine arguments vs initializer
public record ObjectCreationExpression(IType Type, PropertyCollection? Arguments, InitializerExpression? Initializer, Microsoft.CodeAnalysis.IOperation? Operation = null) : Expression<ObjectCreationExpressionSyntax>(Type)
{
    public override ObjectCreationExpressionSyntax Syntax() => ObjectCreationExpression(Type.Syntax(), Arguments?.ToArguments(), Initializer?.Syntax());
    //public  ImplicitObjectCreationExpressionSyntax ImplicitSyntax() => ImplicitObjectCreationExpression(Arguments?.ToArguments(), Initializer?.Syntax());
    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
    }

    public override IExpression Evaluate(IProgramModelExecutionContext context)
    {
        if (Operation is IObjectCreationOperation objectCreationOperation)
        {
            var constructor = SemanticReflection.GetConstructor(objectCreationOperation);
            var value = constructor.Invoke(Arguments is null ? Array.Empty<IExpression>()
                : Arguments.ToExpressions().Select(x => x.EvaluatePlain(context)).ToArray());
            if (Initializer is not null)
            {
                context.EnterScope(value);
                var initialValues = Initializer.EvaluatePlain(context);
                if (initialValues is IEnumerable<object?> initialPlainValues)
                {
                    if (value is System.Collections.IDictionary dictionary)
                    {
                        foreach (var v in initialPlainValues)
                        {
                            if (v is object[] array) dictionary.Add(array[0], array[1]);
                        }
                    }
                    else if (value is System.Collections.IEnumerable collection)
                    {
                        var addMethod = collection.GetType().GetMethod("Add");
                        if (addMethod is null) throw new NotImplementedException($"Unhandled ienumerable: '{value}'.");
                        foreach (var v in initialPlainValues) addMethod.Invoke(collection, new[] { v });
                    }
                }
                var h = new HashSet<int>();
                context.ExitScope(value);
            }
            return Value(value);

        }
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }
}
