using System;
using System.Collections.Generic;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Invocation;
using CodeModels.AbstractCodeModels.Collection;

namespace CodeModels.Models.Primitives.Member;

public record Constructor(IType ReturnType, NamedValueCollection Parameters, Block? Body, IExpression? ExpressionBody = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
    : MethodBase<ConstructorDeclarationSyntax, ConstructorInvocationExpression>(ReturnType, "Constructor", Attributes ?? new List<AttributeList>(), Modifier),
    IConstructor, IInvokable<ConstructorInvocationExpression>
{
    public static Constructor Create(IType type, NamedValueCollection parameters, Block? body = null, IExpression? expressionBody = null, Modifier modifier = Modifier.Public, List<AttributeList>? attributes = null)
        => new(type, parameters, body, expressionBody, modifier, attributes);
    public static Constructor Create(IBaseTypeDeclaration type, NamedValueCollection parameters, Block? body = null, IExpression? expressionBody = null, Modifier modifier = Modifier.Public, List<AttributeList>? attributes = null)
        => new(type.Get_Type(), parameters, body, expressionBody, modifier, attributes)
        {
            Owner = type
        };
    public static Constructor Create(IType returnType, NamedValueCollection parameters, Block body, Modifier modifier = Modifier.Public)
        => Create(returnType, parameters, body, null, modifier);
    public static Constructor Create(IType returnType, NamedValueCollection parameters, IExpression? body = null, Modifier modifier = Modifier.Public)
        => Create(returnType, parameters, null, body, modifier);
    public static Constructor Create(IBaseTypeDeclaration type, NamedValueCollection parameters, Block body, Modifier modifier = Modifier.Public)
        => Create(type, parameters, body, null, modifier);
    public static Constructor Create(IBaseTypeDeclaration type, NamedValueCollection parameters, IExpression? body = null, Modifier modifier = Modifier.Public)
        => Create(type, parameters, null, body, modifier);

    public ConstructorDeclarationSyntax ToConstructorSyntax(Modifier modifiers = Modifier.None, Modifier removeModifier = Modifier.None)
        => ConstructorDeclarationCustom(
        attributeLists: List<AttributeListSyntax>(),
        modifiers: Modifier.SetModifiers(modifiers).SetFlags(removeModifier, false).Syntax(),
        identifier: ToIdentifier(),
        parameterList: Parameters.ToParameters(),
        body: Body?.Syntax(),
        initializer: null,
        expressionBody: ExpressionBody is null ? null : ArrowExpressionClause(ExpressionBody.Syntax()));

    public override ConstructorDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => ToConstructorSyntax(modifier, removeModifier);

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var property in Parameters.Properties) yield return property;
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

    public override ParameterSyntax ToParameter()
    {
        throw new NotImplementedException();
    }

    public override TupleElementSyntax ToTupleElement()
    {
        throw new NotImplementedException();
    }
}
