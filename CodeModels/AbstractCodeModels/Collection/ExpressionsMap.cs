using System.Collections.Generic;
using System.Linq;
using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Instantiation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;

namespace CodeModels.AbstractCodeModels.Collection;

public record ExpressionsMap(IExpression Key, List<IExpression> Values, bool MultiValues = false, IType? ValueType = null) : ExpressionCollection(Values, ValueType)
{
    public ExpressionsMap(IExpression key, IEnumerable<IExpression> values, bool multiValues = false, IType? valueType = null)
        : this(key, values.ToList(), multiValues || values.Count() is not 1, valueType) { }

    public ExpressionsMap(IExpression key, IExpression value, bool multiValues = false, AbstractType? valueType = null)
        : this(key, List(value), multiValues, valueType) { }

    public ExpressionsMap(IExpression key, string commaSeparatedValues) : this(key, commaSeparatedValues.Trim().Split(',').Select(Literal)) { }
    public ExpressionsMap(IExpression key, EnumDeclarationSyntax declaration) : this(key, declaration.Members.Select(Literal)) { }
    public InitializerExpressionSyntax ToKeyValueInitializationSyntax() => ComplexElemementExpressionCustom(
        new ExpressionSyntax[] { Key.Syntax(), MultiValues ? ToArrayInitialization() : Values.First().Syntax() });

    public InitializerExpression ToKeyValueInitialization() => ComplexElementInitializer(new IExpression[] { Key }
        .Concat(new IExpression[] { MultiValues ? ObjectCreation(Type, null, ArrayInitializer(Values, Type)) : Values.First() }), ValueType);

    public override IType BaseType() => ValueType ?? (MultiValues ? base.BaseType().ToMultiType() : base.BaseType());
}


