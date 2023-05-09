﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Factory.CodeModelFactory;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public record ExpressionsMap(IExpression Key, List<IExpression> Values, bool MultiValues = false, IType? ValueType = null) : ExpressionCollection(Values, ValueType)
{
    public ExpressionsMap(IExpression key, IEnumerable<IExpression> values, bool multiValues = false, IType? valueType = null)
        : this(key, values.ToList(), multiValues || (values.Count() is not 1), valueType) {  }

    public ExpressionsMap(IExpression key, IExpression value, bool multiValues = false, AbstractType? valueType = null)
        : this(key, List(value), multiValues, valueType) { }

    public ExpressionsMap(IExpression key, string commaSeparatedValues) : this(key, commaSeparatedValues.Trim().Split(',').Select(x => new LiteralExpression(x))) { }
    public ExpressionsMap(IExpression key, EnumDeclarationSyntax declaration) : this(key, declaration.Members.Select(x => new LiteralExpression(x))) { }
    public InitializerExpressionSyntax ToKeyValueInitialization() => ComplexElemementExpressionCustom(
        new ExpressionSyntax[] { Key.Syntax(), MultiValues ? ToArrayInitialization() : Values.First().Syntax() });

    public override IType BaseType() => ValueType ?? (MultiValues ? base.BaseType().ToMultiType() : base.BaseType());
}


