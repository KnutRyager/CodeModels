using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.DataStructures
{
    /// <summary>
    /// When enums aren't powerful enough, use this for a class of limited instances,
    /// where each instance can be found in a static dictionary.
    /// </summary>
    public abstract class ClassType<T> : IEquatable<T> where T : ClassType<T>
    {
        private static IDictionary<string, T>? _allTypes;

        public string Name { get; }

        protected ClassType(string name)
        {
            Name = name;
        }

#pragma warning disable CA1000
#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8619
        public static IDictionary<string, T> AllTypes()
#pragma warning restore CA1000
        {
            if (_allTypes != null) return _allTypes;

            var types = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.PropertyType == typeof(T))
                .Select(x => (T)x.GetValue(null))
                .ToList();
#if DEBUG
            var duplicates = types.GroupBy(x => x.Name).Where(x => x.Count() > 1).ToList();
            if (duplicates.Any())
            {
                throw new Exception($"Duplicate names: {string.Join(", ", duplicates.Select(x => x.Key))}");
            }
#endif
            _allTypes = types.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            return _allTypes;
        }
#pragma warning restore CS8600
#pragma warning restore CS8602
#pragma warning restore CS8619

#pragma warning disable CA1000
        public static T? FromString(string? name) => string.IsNullOrEmpty(name) ? null : AllTypes().TryGetValue(name, out var value) ? value : null;
        public bool Equals(T? other) => other != null && (ReferenceEquals(this, other) || other.Equals(Name));
        public new bool Equals(object other) => other != null && (ReferenceEquals(this, other) || other.Equals(Name));
        public bool Equals(string name) => name != null && Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        public override string ToString() => Name;
    }
}