using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Common.Util;

namespace Common.Reflection;

public static class ReflectionUtil
{
    public const BindingFlags PublicDeclared = BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static;

    public static List<TAttribute> GetPropertyAttributes<T, TAttribute>() where TAttribute : Attribute => GetPropertyAttributes<TAttribute>(typeof(T));
    public static List<TAttribute> GetPropertyAttributes<TAttribute>(Type type) where TAttribute : Attribute
        => type.GetProperties()
            .Select(x => x.GetCustomAttribute(typeof(TAttribute), true) as TAttribute)
            .Where(x => x != null).ToList()!;

    private static readonly IDictionary<Type, IList<PropertyInfo>> _getPropertiesOfDisplayNameCache = new Dictionary<Type, IList<PropertyInfo>>();
    public static IList<PropertyInfo> GetPropertiesOfDisplayName<T>() => GetPropertiesOfDisplayName(typeof(T));
    public static IList<PropertyInfo> GetPropertiesOfDisplayName(Type type)
    {
        var displayNames = GetPropertyAttributes<DisplayNameAttribute>(type).Select(x => x.DisplayName).ToList();
        if (_getPropertiesOfDisplayNameCache.ContainsKey(type)) return _getPropertiesOfDisplayNameCache[type];
        var result = displayNames.Select(x => GetPropertyOfDisplayName(type, x)).ToArray();
        _getPropertiesOfDisplayNameCache[type] = result;
        return result;
    }

    public static PropertyInfo GetPropertyOfDisplayName<T>(string displayName) => GetPropertyOfDisplayName(typeof(T), displayName);
    public static PropertyInfo GetPropertyOfDisplayName(Type type, string displayName)
        => type.GetProperties().FirstOrDefault(x => x.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName == displayName)!;

    public static bool Implements(Type type, Type @interface) => type.GetInterfaces().Any(x =>
    x == @interface
    || x.IsGenericType &&
        x.GetGenericTypeDefinition() == @interface);

    public static bool Implements<T, TInterface>() => Implements(typeof(T), typeof(TInterface));
    public static bool Implements<T>(T obj, Type @interface) => obj != null && Implements(obj.GetType(), @interface);

    public static MethodInfo GetMethodInfo<TOut>(Expression<Func<TOut>> expression)
        => ((MethodCallExpression)expression.Body).Method;
    public static MethodInfo GetMethodInfo<T>(string name)
        => GetMethodInfo(typeof(T), name, Array.Empty<Type>())!;

    public static MethodInfo GetMethodInfo(Type type, string name)
        => type.GetMethod(name, Array.Empty<Type>())!;

    public static MethodInfo? GetMethodInfo<T>(string name, Type[] parameters)
        => GetGenericMethod<T>(name, parameters: GetParameters(parameters), genericArguments: GetGenericParameters(parameters), genericIndexOfParameters: MapGenericParameters(GetParameters(parameters)));

    public static MethodInfo? GetMethodInfo(Type type, string name, Type[] parameters)
       => GetGenericMethod(type, name, parameters: GetParameters(parameters), genericArguments: GetGenericParameters(parameters), genericIndexOfParameters: MapGenericParameters(GetParameters(parameters)));

    public static MethodInfo GetMethodInfo<T, T1>(string name) => GetMethodInfo<T>(name, parameters: Types<T1>())!;
    public static MethodInfo GetMethodInfo<T, T1, T2>(string name) => GetMethodInfo<T>(name, parameters: Types<T1, T2>())!;
    public static MethodInfo GetMethodInfo<T, T1, T2, T3>(string name) => GetMethodInfo<T>(name, parameters: Types<T1, T2, T3>())!;
    public static MethodInfo GetMethodInfo<T, T1, T2, T3, T4>(string name) => GetMethodInfo<T>(name, parameters: Types<T1, T2, T3, T4>())!;
    public static MethodInfo GetMethodInfo<T, T1, T2, T3, T4, T5>(string name) => GetMethodInfo<T>(name, parameters: Types<T1, T2, T3, T4, T5>())!;

    private static (int Param, int? GenericParam)[] MapGenericParameters(Type[] parameters)
    {
        var results = new List<(int Param, int? GenericParam)>();
        for (var i = 0; i < parameters.Length; i++)
        {
            results.Add((Param: i, GenericParam: GetGenericParamIndex(parameters[i])));
        }
        return results.ToArray();
    }

    public static int? GetGenericParamIndex(Type type) => type switch
    {
        _ when type == typeof(IGenericParameter0) => 0,
        _ when type == typeof(IGenericParameter1) => 1,
        _ when type == typeof(IGenericParameter2) => 2,
        _ when type == typeof(IGenericParameter3) => 3,
        _ when type == typeof(IGenericParameter4) => 4,
        _ when type == typeof(IGenericParameter5) => 5,
        _ => null
    };

    public static MethodInfo GetMethodInfo<T1, TOut>(Expression<Func<T1, TOut>> expression)
        => ((MethodCallExpression)expression.Body).Method;

    public static MethodInfo GetMethodInfo<T1, T2, TOut>(Expression<Func<T1, T2, TOut>> expression)
        => ((MethodCallExpression)expression.Body).Method;

    public static MethodInfo GetMethodInfo<T1, T2, T3, TOut>(Expression<Func<T1, T2, T3, TOut>> expression)
        => ((MethodCallExpression)expression.Body).Method;

    public static MethodInfo GetMethodInfo<T1, T2, T3, T4, TOut>(Expression<Func<T1, T2, T3, T4, TOut>> expression)
        => ((MethodCallExpression)expression.Body).Method;

    public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, TOut>(Expression<Func<T1, T2, T3, T4, T5, TOut>> expression)
        => ((MethodCallExpression)expression.Body).Method;

    public interface IGenericParameterDescriptior { }
    public interface IGenericParameter<T> { }
    public interface IGenericParameter0 : IGenericParameterDescriptior { }
    public interface IGenericParameter1 : IGenericParameterDescriptior { }
    public interface IGenericParameter2 : IGenericParameterDescriptior { }
    public interface IGenericParameter3 : IGenericParameterDescriptior { }
    public interface IGenericParameter4 : IGenericParameterDescriptior { }
    public interface IGenericParameter5 : IGenericParameterDescriptior { }

    public static MethodInfo[] GetStaticMethods<T>()
        => typeof(T).GetMethods(BindingFlags.Static | BindingFlags.Public);

    public static MethodInfo? GetStaticMethodInfo<T>(string name)
        => GetStaticMethods<T>().Where(x => x.Name == name).FirstOrDefault();

    public static MethodInfo? GetStaticMethodInfo<T, T1>(string name)
        => GetStaticMethods<T>().Where(x => x.Name == name
        && IsSameParamemters(x.GetParameters(), typeof(T1))).FirstOrDefault();

    public static MethodInfo? GetStaticMethodInfo<T, T1, T2>(string name)
        => GetStaticMethods<T>().Where(x => x.Name == name
        && IsSameParamemters(x.GetParameters(), Types<T1, T2>())).FirstOrDefault();

    public static MethodInfo? GetStaticMethodInfo<T, T1, T2, T3>(string name)
        => GetStaticMethods<T>().Where(x => x.Name == name
        && IsSameParamemters(x.GetParameters(), Types<T1, T2, T3>())).FirstOrDefault();

    public static MethodInfo? GetStaticMethodInfo<T, T1, T2, T3, T4>(string name)
        => GetStaticMethods<T>().Where(x => x.Name == name
        && IsSameParamemters(x.GetParameters(), Types<T1, T2, T3, T4>())).FirstOrDefault();

    public static MethodInfo? GetStaticMethodInfo<T, T1, T2, T3, T4, T5>(string name)
        => GetStaticMethods<T>().Where(x => x.Name == name
        && IsSameParamemters(x.GetParameters(), Types<T1, T2, T3, T4, T5>())).FirstOrDefault();

    public static bool IsGenericParameter(Type type) => type.FullName is null || type.Name.StartsWith("IGenericParameter`");
    public static bool IsGenericParameterDescriptor(Type type) => type.Name.StartsWith("IGenericParameter") && !IsGenericParameter(type);
    public static Type[] GetGenericParameters(IEnumerable<Type> types) => types.Where(IsGenericParameter).Select(x => x.GenericTypeArguments.First()).ToArray();
    
    /// <summary>
    /// Filter away those generic types that are of the IGenericParameter interface.
    /// </summary>
    public static Type[] GetParameters(IEnumerable<Type> types) => types.Where(x => !IsGenericParameter(x)).ToArray();

    private static bool IsSameParamemters(ParameterInfo[] parameters, params Type[] types)
    {
        if (parameters.Length != types.Length) return false;
        for (var i = 0; i < parameters.Length; i++) if (parameters[i].ParameterType != types[i]) return false;
        return true;
    }

    private static readonly TypeCode[] NumberTypes = new[] {
            TypeCode.Byte, TypeCode.SByte, TypeCode.UInt16,TypeCode.UInt32,TypeCode.UInt64,TypeCode.Int16,
            TypeCode.Int32,TypeCode.Int64,TypeCode.Decimal,TypeCode.Double,TypeCode.Single
        };

    public static bool IsNumeric(Type type) => NumberTypes.Contains(Type.GetTypeCode(type));

    public static Attribute? GetPropertyAttribute(Type type, string property, Type attribute) => GetAttribute(type.GetProperty(property), attribute);
    public static T? GetAttribute<T>(PropertyInfo? property) where T : Attribute => GetAttribute(property, typeof(T)) as T;
    public static Attribute? GetAttribute(PropertyInfo? property, Type attribute) => property?.GetCustomAttribute(attribute);

    public static PropertyInfo? GetPropertyOfAttribute<T, TAttribute>(Predicate<TAttribute> predicate) where TAttribute : Attribute
        => GetPropertyOfAttribute<TAttribute>(typeof(T), (Predicate<object>)predicate);

    public static PropertyInfo? GetPropertyOfAttribute<T>(Type type, Predicate<T> predicate) where T : Attribute
    {
        foreach (var property in type.GetProperties())
        {
            var attributeFound = (T?)property.GetCustomAttribute(typeof(T));
            if (attributeFound == null) continue;
            if (predicate(attributeFound)) return property;
        }
        return null;
    }

    public static FieldInfo? GetField<T>(string name) => GetField(typeof(T), name);
    public static FieldInfo? GetField(Type type, string name) => type.GetField(name);

    public static object? GetPropertyValueOfAttribute<T, TAttribute>(Predicate<TAttribute> predicate, T instance) where TAttribute : Attribute
        => GetPropertyValueOfAttribute(typeof(T), predicate, instance);

    public static object? GetPropertyValueOfAttribute<T, TAttribute>(Type type, Predicate<TAttribute> predicate, T instance) where TAttribute : Attribute
        => GetPropertyOfAttribute(type, predicate)?.GetValue(instance);

    public static MethodInfo? GetOrMakeGenericMethod<T, TMethodType>(string name, Type[]? parameters = null, int? parameterCount = null, (int Param, int? GenericParam)? genericIndexOfParameter = null)
        => GetOrMakeGenericMethod<TMethodType>(typeof(T), name, parameterCount, parameters, genericIndexOfParameter);

    public static MethodInfo? GetOrMakeGenericMethod<TMethodType>(Type type, string name, int? parameterCount = null, Type?[]? parameters = null, (int Param, int? GenericParam)? genericIndexOfParameter = null)
        => GetOrMakeGenericMethod(type, name, parameterCount, 1, parameters, new Type[] { typeof(TMethodType) }, genericIndexOfParameter == null ? null : new (int Param, int? GenericParam)[] { genericIndexOfParameter.Value });

    public static MethodInfo? GetOrMakeGenericMethod(Type type, string name, int? parameterCount = null, int? genericArgumentCount = null, Type?[]? parameters = null, Type[]? genericArguments = null, (int Param, int? GenericParam)[]? genericIndexOfParameters = null)
        => (GetGenericMethod(type, name, parameterCount, genericArgumentCount, parameters, genericArguments, genericIndexOfParameters) ?? type.GetMethod(name))?.MakeGenericMethod(genericArguments ?? Array.Empty<Type>());

    public static TMethodType RunGenericMethod<T, TMethodType>(string methodName, T? instance = null, params object[] parameters) where T : class
        => (TMethodType)GetOrMakeGenericMethod<T, TMethodType>(methodName)?.Invoke(instance, parameters)!;

    public static TMethodType RunStaticGenericMethod<TMethodType>(Type type, string methodName, params object[] parameters)
        => (TMethodType)GetOrMakeGenericMethod<TMethodType>(type, methodName)?.Invoke(null, parameters)!;

    public static MethodInfo? GetGenericMethod<T>(string name, int? parameterCount = null, int? genericArgumentCount = null, Type?[]? parameters = null, Type[]? genericArguments = null, (int Param, int? GenericParam)[]? genericIndexOfParameters = null)
        => GetGenericMethods<T>(name, parameterCount, genericArgumentCount, parameters, genericArguments, genericIndexOfParameters)?.FirstOrDefault();

    public static MethodInfo? GetGenericMethod(Type type, string name, int? parameterCount = null, int? genericArgumentCount = null, Type?[]? parameters = null, Type[]? genericArguments = null, (int Param, int? GenericParam)[]? genericIndexOfParameters = null)
        => GetGenericMethods(type, name, parameterCount, genericArgumentCount, parameters, genericArguments, genericIndexOfParameters)?.FirstOrDefault();

    public static MethodInfo[] GetGenericMethods<T>(string name, int? parameterCount = null, int? genericArgumentCount = null, Type?[]? parameters = null, Type[]? genericArguments = null, (int Param, int? GenericParam)[]? genericIndexOfParameters = null)
        => GetGenericMethods(typeof(T), name, parameterCount, genericArgumentCount, parameters, genericArguments, genericIndexOfParameters);

    public static MethodInfo[] GetGenericMethods(Type type, string name, int? parameterCount = null, int? genericArgumentCount = null, Type?[]? parameters = null, Type[]? genericArguments = null, (int Param, int? GenericParam)[]? genericIndexOfParameters = null)
    {
        parameterCount ??= parameters?.Length;
        genericArgumentCount ??= genericArguments?.Length;
        return type
         .GetMethods()
         .Where(m => m.Name == name)
         .Select(m => new
         {
             Method = m,
             Params = m.GetParameters(),
             Args = m.GetGenericArguments()
         })
         .Where(x =>
             (parameterCount == null || x.Params.Length == parameterCount)
             && (genericArgumentCount == null || x.Args.Length == genericArgumentCount)
             && (genericIndexOfParameters == null || genericIndexOfParameters.All(y => y.GenericParam == null || (x.Params.Length > y.Param
                && IsSameType(y.GenericParam.Value, x.Args[y.GenericParam.Value], x.Params[y.Param].ParameterType, parameters![y.GenericParam.Value]!))))
             && (parameters == null || IsMethodMatch(x.Method, parameters!, genericArguments))
             )
         .Select(x => x.Method)
         .ToArray();
    }

    private static bool IsMethodMatch(MethodInfo method, Type[] parameters, Type[]? genericArguments = null)
    {
        MethodInfo? genericMethod = null;
        if (method.IsGenericMethod || method.ContainsGenericParameters)
            try
            {
                genericMethod = method.GetGenericMethodDefinition();
            }
            catch (Exception) { }
        var methodParameters = method.GetParameters();
        if (parameters.Length != methodParameters.Length)
        {
            return false;
        }
        var genericParameters = genericMethod?.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            if (!IsParameterMatch(parameters[i], methodParameters[i], genericParameters is null ? null : genericParameters[i])) return false;
        }

        return true;
    }

    private static bool IsParameterMatch(Type parameter, ParameterInfo parameter2, ParameterInfo? genericParameter = null)
        => IsTypeMatch(parameter, parameter2.ParameterType, genericParameter);

    private static bool IsTypeMatch(Type type, Type type2, ParameterInfo? genericParameter = null)
    {
        if (type == type2) return true;
        var classification2 = ReflectionSerialization.Classify(type2);
        if (classification2 == TypeVariant.GenericUnbound) return true;
        var genericParameters = type.GetGenericArguments();
        var genericParameters2 = type2.GetGenericArguments();
        if (genericParameters.Length != genericParameters2.Length || genericParameters.Length is 0) return false;
        for (var i = 0; i < genericParameters.Length; i++)
        {
            if (!IsTypeMatch(genericParameters[i], genericParameters2[i], genericParameter)) return false;
        }
        return true;
    }

    public static bool IsDirectlyGenericType(TypeVariant variant) => variant == TypeVariant.GenericUnbound || variant == TypeVariant.GenericBound;

    private static bool IsSameType(int genericParamIndex, Type genericParam, Type param, Type argument)
        => genericParam == param && genericParamIndex == GetGenericParamIndex(argument);

    public static PropertyInfo? GetFieldOfType<TModel, TFieldType>() => GetFieldOfType(typeof(TModel), typeof(TFieldType));

    public static PropertyInfo? GetFieldOfType(Type type, Type fieldType)
        => type.GetProperties().FirstOrDefault(x => x.PropertyType == fieldType);

    public static TFieldType GetFieldValueOfType<TModel, TFieldType>(TModel instance)
        => (TFieldType)GetFieldOfType<TModel, TFieldType>()?.GetGetMethod()?.Invoke(instance, null)!;

    /// <summary>
    /// Get the default value for a type.
    /// </summary>
    public static T GetDefault<T>() => (T)GetDefault(typeof(T))!;

    /// <summary>
    /// Get the default value for a type.
    /// </summary>
    public static object? GetDefault(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;

    /// <summary>
    /// Copies non-null/0 (default) values from source to target.
    /// 
    /// Based on https://stackoverflow.com/questions/13212498/loop-through-an-object-and-find-the-not-null-properties
    /// </summary>
    public static void CopyNonDefaultProperties<T>(T source, T target) where T : notnull, new()
    {
        foreach (var prop in GetReadWriteProperties(source.GetType()))
        {
            var value = prop.GetValue(source, null);
            if (value != GetDefault(prop.PropertyType))
            {
                prop.SetValue(target, value, null);
            }
        }
    }

    public static PropertyInfo[] GetReadWriteProperties(Type type)
        => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => !p.GetIndexParameters().Any())
            .Where(p => p.CanRead && p.CanWrite).ToArray();

    public static T ConvertTo<T>(object value)
    {
        if (value is T variable) return variable;
        try
        {
            return GetUnderlyingType(typeof(T)) != null
                ? (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value)!
                : (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception)
        {
            return default!;
        }
    }

    private static readonly IDictionary<Type, Type?> _getUnderlyingTypeCache = new Dictionary<Type, Type?>();
    public static Type? GetUnderlyingType(Type type)
    {
        if (_getUnderlyingTypeCache.ContainsKey(type)) return _getUnderlyingTypeCache[type];
        var underlyingType = Nullable.GetUnderlyingType(type);
        _getUnderlyingTypeCache[type] = underlyingType;
        return underlyingType;
    }
    public static Type GetUnderlyingTypeOrBase(Type type) => GetUnderlyingType(type) ?? type;

    //public static bool IsNullable(PropertyInfo property) => IsNullable(property.PropertyType, property);
    //public static bool IsNullable(Type type, PropertyInfo? property = null)
    //{
    //    return GetUnderlyingType(type) != null || (type == typeof(string) && (property == null || property.GetCustomAttribute<RequiredAttribute>() == null));
    //}

    public static List<Type> GetMemberTypes<T>() => GetMemberTypes(typeof(T));
    public static List<Type> GetMemberTypes(Type type)
        => type.GetMembers(BindingFlags.Public).Select(x => (Type)x).ToList();

    public class Placeholder { }

    public static bool IsTypeCompatibile<TPattern, TTest, TPlaceholder>() => IsTypeCompatibile(typeof(TPattern), typeof(TTest), typeof(TPlaceholder));
    public static bool IsTypeCompatibile(Type pattern, Type test, Type? placeholder = null)
    {
        if (placeholder == null) placeholder = typeof(Placeholder);
        if (pattern == test || pattern == placeholder)
            return true;

        if (pattern.IsGenericType)
        {
            if (!test.IsGenericType || pattern.GetGenericTypeDefinition() != test.GetGenericTypeDefinition())
                return false;

            var patternGenericTypes = pattern.GetGenericArguments();
            var testGenericTypes = test.GetGenericArguments();

            return patternGenericTypes.Length == testGenericTypes.Length
                && patternGenericTypes.Zip(testGenericTypes, (p, t) => IsTypeCompatibile(p, t, placeholder)).All(x => x);

        }

        return false;
    }

    public static int IndexOfProperty<T>(string name) => IndexOfProperty(typeof(T), name);
    public static int IndexOfProperty(Type type, string name) => type.GetProperties().TakeWhile(x => x.Name != name).Count();

    public static Type[] Types<T>() => new[] { typeof(T) };
    public static Type[] Types<T1, T2>() => new[] { typeof(T1), typeof(T2) };
    public static Type[] Types<T1, T2, T3>() => new[] { typeof(T1), typeof(T2), typeof(T3) };
    public static Type[] Types<T1, T2, T3, T4>() => new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
    public static Type[] Types<T1, T2, T3, T4, T5>() => new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
    public static Type[] Types<T1, T2, T3, T4, T5, T6>() => new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };

    public static Assembly[] GetAssembies() => AppDomain.CurrentDomain.GetAssemblies();
    public static Assembly? GetAssembly(string name) => GetAssembies().FirstOrDefault(x => x.GetName().FullName.Contains(name));
    // TODO: Return something for unloadable assemblies?
    public static Assembly[] GetDirectDependencies(Assembly assembly) => assembly.GetReferencedAssemblies().Select(x => LoadAssembly(x)!).Where(x => x != null).ToArray();
    public static List<Assembly> GetAllDependencies(Assembly assembly)
    {
        var dependencies = GetDirectDependencies(assembly);
        var allDependencies = new List<Assembly>(dependencies);
        var checkedDependencies = new HashSet<Assembly>(dependencies);
        var checkQueue = new Queue<Assembly>(dependencies.SelectMany(GetDirectDependencies));
        while (checkQueue.Any())
        {
            var dependency = checkQueue.Dequeue();
            if (checkedDependencies.Contains(dependency)) continue;
            allDependencies.Add(dependency);
            GetDirectDependencies(dependency).E_ForEach(x => checkQueue.Enqueue(x));
            checkedDependencies.Add(dependency);
        }
        return allDependencies;
    }

    private static Assembly? LoadAssembly(AssemblyName assembly)
    {
        try
        {
            return Assembly.Load(assembly);
        }
        catch (Exception) { }
        return null;
    }

    public static IEnumerable<Type> GetAllLoadedTypes() => GetAssembies().SelectMany(x => GetTypes(x));

    public static IEnumerable<Type> GetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (Exception)
        {
            return Array.Empty<Type>();
        }
    }

    public static IEnumerable<(Assembly Assembly, IEnumerable<Type> Types)> GetTypes(IEnumerable<Assembly> assemblies)
        => assemblies.Select(x => (x, GetTypes(x)));

    public static IEnumerable<string> GetNamespaces(Assembly assembly)
        => GetTypes(assembly).Select(x => x.Namespace!).Distinct().Where(x => x != null);

    public static IEnumerable<(Assembly Assembly, IEnumerable<string> Namespaces)> GetNamespaces(IEnumerable<Assembly> assemblies)
        => assemblies.Select(x => (x, GetNamespaces(x)));

    public static IEnumerable<Type> FindLoadedTypes(string? @namespace = null) => from t in GetAllLoadedTypes()
                                                                                  .Where(x => @namespace == null || (x.Namespace?.ToLower() ?? "").Contains(@namespace))
                                                                                  select t;
    public static List<string> FindLoadedNamespaces(string @namespace)
    {
        @namespace = @namespace.ToLower();
        var parts = @namespace.Split(".".ToCharArray());
        var lastDotIndex = @namespace.LastIndexOf(".");
        var firstParts = lastDotIndex < 0 ? null : @namespace[..lastDotIndex];
        var namespaces = FindLoadedTypes(@namespace).Select(x => x.Namespace).Distinct().Where(x => x != null && x.ToLower()
        .StartsWith(@namespace)).Where(x => x!.Length >= lastDotIndex)
        .Select(x =>
        {
            var partAfterDot = x![(lastDotIndex + 1)..];
            var nextDotIndex = partAfterDot.IndexOf(".");
            var dotIndex = lastDotIndex < 0 ? x!.IndexOf(".") : nextDotIndex < 0 ? x.Length : nextDotIndex + lastDotIndex + 1;
            return dotIndex >= x.Length || dotIndex < 0 ? x : x![..dotIndex];
        })
        .Distinct();
        return namespaces.ToList();
    }

    public static bool IsPrimitiveType(Type type) => type switch
    {
        _ when type == typeof(bool) => true,
        _ when type == typeof(byte) => true,
        _ when type == typeof(sbyte) => true,
        _ when type == typeof(char) => true,
        _ when type == typeof(short) => true,
        _ when type == typeof(ushort) => true,
        _ when type == typeof(int) => true,
        _ when type == typeof(uint) => true,
        _ when type == typeof(nint) => true,
        _ when type == typeof(nuint) => true,
        _ when type == typeof(long) => true,
        _ when type == typeof(ulong) => true,
        _ when type == typeof(float) => true,
        _ when type == typeof(double) => true,
        _ when type == typeof(decimal) => true,
        _ when type == typeof(string) => true,
        _ => false
    };

    public static bool IsContainerOfPrimitiveType(Type type) => IsContainerOf(type, IsPrimitiveType);
    public static bool IsContainerOf(Type type, Func<Type, bool> predicate) => typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type.GetGenericArguments().Any(predicate);
    public static bool IsStatic(Type type) => type.IsAbstract && type.IsSealed && !type.GetConstructors().Any();
    public static bool IsStatic(PropertyInfo property) => property.GetAccessors(true)[0].IsStatic;
}
