using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Common.Factory;

public interface IFactory<T> where T : class, new()
{
    T Default { get; set; }
    T New { get; }

    IFactory<T> Set<TNavProp, TProp>(Expression<Func<T, ICollection<TNavProp>>> navigationProperty, Expression<Func<TNavProp, TProp>> property, IEnumerable<TProp> values) where TNavProp : new();
    IFactory<T> Set<TNavProp, TProp>(Expression<Func<T, ICollection<TNavProp>>> navigationProperty, Expression<Func<TNavProp, TProp>> property, TProp value) where TNavProp : new();
    IFactory<T> Set<TProp>(Expression<Func<T, TProp>> property, TProp value);
}
