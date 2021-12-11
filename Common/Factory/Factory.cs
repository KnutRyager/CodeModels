using Common.Util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Common.Factory
{
    public class Factory<T> : IFactory<T> where T : class, new()
    {
        private T _default;

        public Factory(T? @default = default)
        {
            _default = @default ?? New;
        }

        public T New { get => new(); }
        public T Default { get => CopyUtil.Copy(_default); set => _default = CopyUtil.Copy(value); }

        public IFactory<T> Set<TProp>(Expression<Func<T, TProp>> property, TProp value)
        {
            var setter = ExpressionUtil.MakeSetter(property);
            setter(_default, value);
            return this;
        }

        public IFactory<T> Set<TNavProp, TProp>(Expression<Func<T, ICollection<TNavProp>>> navigationProperty, Expression<Func<TNavProp, TProp>> property, TProp value) where TNavProp : new()
            => Set(navigationProperty, property, new[] { value });

        public IFactory<T> Set<TNavProp, TProp>(Expression<Func<T, ICollection<TNavProp>>> navigationProperty, Expression<Func<TNavProp, TProp>> property, IEnumerable<TProp> values) where TNavProp : new()
        {
            var navSetter = ExpressionUtil.MakeSetter(navigationProperty);
            var propSetter = ExpressionUtil.MakeSetter(property);
            var navigationList = new List<TNavProp>();
            foreach (var value in values)
            {
                var navigation = new TNavProp();
                propSetter(navigation, value);
                navigationList.Add(navigation);
            }
            navSetter(_default, navigationList);
            return this;
        }
    }
}