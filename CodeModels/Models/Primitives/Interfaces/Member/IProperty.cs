using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public interface IProperty : IFieldOrProperty, IScopeHolder
{
    IType Type { get; }
    IPropertyExpression Access(IExpression? instance = null);
}

public interface IProperty<T> : IProperty where T : IPropertyExpression
{
    new T Access(IExpression? instance = null);
}

