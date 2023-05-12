using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public interface IField : IFieldOrProperty, IScopeHolder
{
    IType Type { get; }
    IFieldExpression Access(IExpression? instance = null);
}

public interface IField<T> : IField where T : IFieldExpression
{
    new T Access(IExpression? instance = null);
}

