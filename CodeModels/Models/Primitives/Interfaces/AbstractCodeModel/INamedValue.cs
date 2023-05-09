﻿using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public interface INamedValue : INamed
{
    Modifier Modifier { get; }
    IType Type { get; }
    IExpression Value { get; }
    PropertyModel ToProperty();
}
