using System.Collections.Generic;
using System.Linq;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.AbstractCodeModels.Collection;

public record ExpressionsMap(IExpression Key, List<IExpression> Values, bool MultiValues = false, IType? ValueType = null) : ExpressionCollection(Values, ValueType)
{
    public static ExpressionsMap Create(IExpression key, IEnumerable<IExpression> values, bool multiValues = false, IType? valueType = null)
        => new(key, values.ToList(), multiValues || values.Count() is not 1, valueType);

    public static ExpressionsMap Create(IExpression key, IExpression value, bool multiValues = false, AbstractType? valueType = null)
        => Create(key, List(value), multiValues, valueType);

    public static ExpressionsMap Create(IExpression key, string commaSeparatedValues)
        => Create(key, commaSeparatedValues.Trim().Split(',').Select(Literal));

    public static ExpressionsMap Create(IExpression key, EnumDeclarationSyntax declaration)
        => Create(key, declaration.Members.Select(Literal));

    public InitializerExpressionSyntax ToKeyValueInitializationSyntax() => ComplexElemementExpressionCustom(
        new ExpressionSyntax[] { Key.Syntax(), MultiValues ? ToArrayInitialization() : Values.First().Syntax() });

    public InitializerExpression ToKeyValueInitialization() => ComplexElementInitializer(new IExpression[] { Key }
        .Concat(new IExpression[] { MultiValues ? ObjectCreation(Type, null, ArrayInitializer(Values, Type)) : Values.First() }), ValueType);

    public override IType BaseType() => ValueType ?? (MultiValues ? base.BaseType().ToMultiType() : base.BaseType());
}


