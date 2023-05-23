using System;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Utils;

public static class ExpressionUtils
{
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
    public static bool IsNull(IExpression? e) => e is null || e == CodeModelFactory.NullValue || e == CodeModelFactory.VoidValue;
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast

#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
    public static bool IsVoid(IExpression? e) => e == CodeModelFactory.VoidValue;
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast

    public static IExpression ExpressionOrVoid(IExpression? e) => e ?? CodeModelFactory.VoidValue;
    public static int Add(IExpression e, int value) => e.LiteralValue() is int n ? n + value : throw new ArgumentException($"Not a number: {e}");
}
