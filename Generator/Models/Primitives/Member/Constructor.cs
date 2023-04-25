using System.Collections.Generic;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record Constructor(ITypeDeclaration ClassType, PropertyCollection Parameters, Block? Body, IExpression? ExpressionBody = null,
    Modifier Modifier = Modifier.Public, List<AttributeList>? Attributes = null)
    : MethodBase<ConstructorDeclarationSyntax>(ClassType.Get_Type(), ClassType.Name, Attributes ?? new List<AttributeList>(), Modifier), IConstructor
{
    public Constructor(ITypeDeclaration type, PropertyCollection parameters, Block body, Modifier modifier = Modifier.Public)
        : this(type, parameters, body, null, modifier) { }
    public Constructor(ITypeDeclaration type, PropertyCollection parameters, IExpression? body = null, Modifier modifier = Modifier.Public)
        : this(type, parameters, null, body, modifier) { }

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
    public ConstructorInvocationExpression Invoke(params IExpression[] arguments) => ConstructorInvocation(this,  arguments);
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
        throw new System.NotImplementedException();
    }
}
