using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Invocation;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member;

public record Constructor(IType ReturnType, ParameterList Parameters, Block? Body,
    IExpression? ExpressionBody,
    Modifier Modifier, AttributeListList Attributes, ConstructorInitializer? Initializer)
    : MethodBase<ConstructorDeclarationSyntax, ConstructorInvocationExpression>(ReturnType, "Constructor", Parameters, Attributes ?? AttributesList(), Modifier),
    IConstructor, IInvokable<ConstructorInvocationExpression>
{
    public static Constructor Create(IType type, IToParameterListConvertible? parameters, Block? body = null, IExpression? expressionBody = null, Modifier modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => new(type, parameters?.ToParameterList() ?? ParamList(), body, expressionBody, modifier, attributes ?? AttributesList(), initializer);
    public static Constructor Create(IBaseTypeDeclaration type, IToParameterListConvertible? parameters, Block? body = null, IExpression? expressionBody = null, Modifier modifier = Modifier.Public, AttributeListList? attributes = null, ConstructorInitializer? initializer = null)
        => new(type.Get_Type(), parameters?.ToParameterList() ?? ParamList(), body, expressionBody, modifier, attributes ?? AttributesList(), initializer)
        {
            Owner = type
        };
    public static Constructor Create(IType returnType, IToParameterListConvertible parameters, Block body, Modifier modifier = Modifier.Public, ConstructorInitializer? initializer = null)
        => Create(returnType, parameters?.ToParameterList() ?? ParamList(), body, null, modifier, initializer: initializer);
    public static Constructor Create(IType returnType, IToParameterListConvertible parameters, IExpression? body = null, Modifier modifier = Modifier.Public, ConstructorInitializer? initializer = null)
        => Create(returnType, parameters?.ToParameterList() ?? ParamList(), null, body, modifier, initializer: initializer);
    public static Constructor Create(IBaseTypeDeclaration type, IToParameterListConvertible parameters, Block body, Modifier modifier = Modifier.Public, ConstructorInitializer? initializer = null)
        => Create(type, parameters?.ToParameterList() ?? ParamList(), body, null, modifier, initializer: initializer);
    public static Constructor Create(IBaseTypeDeclaration type, IToParameterListConvertible parameters, IExpression? body = null, Modifier modifier = Modifier.Public, ConstructorInitializer? initializer = null)
        => Create(type, parameters?.ToParameterList() ?? ParamList(), null, body, modifier, initializer: initializer);

    public ConstructorDeclarationSyntax ToConstructorSyntax(Modifier modifiers = Modifier.None, Modifier removeModifier = Modifier.None)
        => ConstructorDeclarationCustom(
        attributeLists: Attributes.Syntax(),
        modifiers: Modifier.SetModifiers(modifiers).SetFlags(removeModifier, false).Syntax(),
        identifier: ToIdentifier(),
        parameterList: Parameters.Syntax(),
        body: Body?.Syntax(),
        initializer: Initializer?.Syntax(),
        expressionBody: ExpressionBody is null ? null : ArrowExpressionClause(ExpressionBody.Syntax()));

    public override ConstructorDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => ToConstructorSyntax(modifier, removeModifier);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Parameters;
        if (Body is not null) yield return Body;
        if (ExpressionBody is not null) yield return ExpressionBody;
    }

    public ConstructorInvocationExpression Invoke(IEnumerable<IExpression> arguments) => ConstructorInvocation(this, arguments);
    public ConstructorInvocationExpression Invoke(params IExpression[] arguments) => ConstructorInvocation(this, arguments);
    public override ConstructorInvocationExpression Invoke(IExpression? _, IEnumerable<IExpression> arguments) => Invoke(arguments);
    //public ConstructorInvocationExpression Invoke(IType? type, ISymbol? symbol, IEnumerable<IExpression> arguments) => Invoke(Identifier( type, symbol), arguments);
    //public ConstructorInvocationExpression Invoke(IType? type, ISymbol? symbol, params IExpression[] arguments) => Invoke(Identifier( type, symbol), arguments);
    //public ConstructorInvocationExpression Invoke(IType type, IEnumerable<IExpression> arguments) => Invoke(Identifier( type), arguments);
    //public ConstructorInvocationExpression Invoke(IType type, params IExpression[] arguments) => Invoke(Identifier( type), arguments);
    //public ConstructorInvocationExpression Invoke(ISymbol symbol, IEnumerable<IExpression> arguments) => Invoke(Identifier( symbol: symbol), arguments);
    //public ConstructorInvocationExpression Invoke(ISymbol symbol, params IExpression[] arguments) => Invoke(Identifier( symbol: symbol), arguments);

    //private Method MakeMethod()
    //{
    //    var block = ExpressionBody is null ? CodeModelFactory.Block(ExpressionBody!) : Body;
    //    block.Add(CodeModelFactory.
    //        Return());
    //    var m = CodeModelFactory.Method(Name, Parameters, new QuickType(Name), Body);
    //}

    public override CodeModel<ConstructorDeclarationSyntax> Render(Namespace @namespace)
    {
        throw new NotImplementedException();
    }

    public override SyntaxToken ToIdentifier() => Identifier(Owner?.Name ?? throw new NotImplementedException());

    public override IType ToType()
    {
        throw new NotImplementedException();
    }
    public override IExpression ToExpression()
    {
        throw new NotImplementedException();
    }

    public override ParameterSyntax ToParameterSyntax()
    {
        throw new NotImplementedException();
    }

    public override TupleElementSyntax ToTupleElement()
    {
        throw new NotImplementedException();
    }
}
