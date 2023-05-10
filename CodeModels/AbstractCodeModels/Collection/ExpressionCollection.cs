using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeModels.Factory.CodeModelFactory;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Factory;
using CodeModels.Models;

namespace CodeModels.AbstractCodeModels.Collection;

public record ExpressionCollection(List<IExpression> Values, IType? SpecifiedType = null)
    : Expression<ArrayCreationExpressionSyntax>(Type(SpecifiedType ?? TypeUtil.FindCommonType(Values), isMulti: true)),
    IExpressionCollection
{
    public ExpressionCollection(IEnumerable<IExpression>? values = null, IType? specifiedType = null) : this(List(values), specifiedType) { }
    public ExpressionCollection(string commaSeparatedValues) : this(commaSeparatedValues.Trim().Split(',').Select(CodeModelFactory.Literal)) { }
    public ExpressionCollection(EnumDeclarationSyntax declaration) : this(declaration.Members.Select(x => CodeModelFactory.Literal(x.Identifier.ToString()))) { }

    public EnumDeclarationSyntax ToEnum(string name, bool isFlags = false, bool hasNoneValue = false) => EnumDeclaration(
            attributeLists: default,
            modifiers: TokenList(Token(SyntaxKind.PublicKeyword)),
            identifier: Identifier(name),
            baseList: default,
            members: SeparatedList(Values.Select((x, index) => x.ToEnumValue(isFlags ? hasNoneValue && index == 0 ? 0 : (int)Math.Pow(2, index - (hasNoneValue ? 1 : 0)) : null)))
        );

    public ArgumentListSyntax ToArguments() => ArgumentListCustom(Values.Select(x => x.ToArgument()));
    public TypeSyntax BaseTypeSyntax() => BaseType().Syntax();
    public virtual IType BaseType() => SpecifiedType ?? TypeUtil.FindCommonType(Values);
    public ArrayCreationExpressionSyntax ToArrayInitialization() => ArrayInitializationCustom(BaseType().TypeSyntaxNonMultiWrapped(), Values.Select(x => x.Syntax()));
    public ObjectCreationExpressionSyntax ToListInitialization() => ListInitializationCustom(BaseTypeSyntax(), Values.Select(x => x.Syntax()));

    public override ArrayCreationExpressionSyntax Syntax() => ToArrayInitialization();

    public override IEnumerable<ICodeModel> Children() => Values;

    public override IExpression Evaluate(ICodeModelExecutionContext context)
    {
        var array = Array.CreateInstance(Type.PlainType().GetReflectedType(), Values.Count);
        for (var i = 0; i < array.Length; i++) { array.SetValue(Values[i].EvaluatePlain(context), i); }
        return Literal(array);
    }

    public List<IType> AsList(IType? typeSpecifier = null) => Values.Select(x => x.Get_Type()).ToList();
    public ArgumentListSyntax As(ArgumentListSyntax? typeSpecifier = null) => ToArguments();

    //public override IExpression Evaluate(IProgramModelExecutionContext context) => Values.Select(x => x.Evaluate(context)).ToArray();
}


