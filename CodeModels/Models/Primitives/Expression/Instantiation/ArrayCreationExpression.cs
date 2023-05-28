using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

// TODO: Multidimensional
public record ArrayCreationExpression(IType Type, List<List<IExpression>>? Arguments, InitializerExpression? Initializer, Microsoft.CodeAnalysis.IOperation? Operation = null)
    : Expression<ArrayCreationExpressionSyntax>(Type)
{
    public static ArrayCreationExpression Create(IType type,
        IEnumerable<List<IExpression>>? arguments = null,
        InitializerExpression? initializer = null,
        Microsoft.CodeAnalysis.IOperation? operation = null)
        => new(type, arguments is null ? null : List(arguments), initializer, operation);

    public override ArrayCreationExpressionSyntax Syntax()
        => ArrayCreationExpression(Type.Syntax() is ArrayTypeSyntax arrayType ? arrayType : ArrayType(Type.Syntax()), Initializer?.Syntax());

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        if (Arguments is not null)
        {
            foreach (var argument in Arguments)
            {
                foreach (var innerArgument in argument)
                    yield return innerArgument;
            }
        }
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        IExpression? value;
        if (Operation is IObjectCreationOperation objectCreationOperation && SymbolUtils.IsNewDefined(objectCreationOperation))
        {
            var member = context.ProgramContext.Get<IClassDeclaration>(objectCreationOperation.Type ?? throw new NotImplementedException());
            var constructor = member.GetConstructor();
            var arguments = Arguments is null ? Array.Empty<IExpression>()
               : Arguments.First().ToArray();
            var invocation = ConstructorInvocation(constructor, arguments);
            value = invocation.Evaluate(context);
        }
        if (Initializer is not null)
        {
            value = Initializer.Evaluate(context);
        }
        else
        {
            var constructor = GetConstructor();
            value = Value(constructor.Invoke(Arguments is null
                ? new object[] { 0 }
               : Arguments.First().Select(x => x.EvaluatePlain(context)).ToArray()));
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
            var parameters = Arguments?.FirstOrDefault()?.Select(x => x.Get_Type().ReflectedType).ToArray() ?? new Type[] { typeof(int) };
            var constructor = type?.GetConstructor(parameters);
            if (constructor is not null)
            {
                return constructor;
            }
        }
        throw new NotImplementedException();
    }
}