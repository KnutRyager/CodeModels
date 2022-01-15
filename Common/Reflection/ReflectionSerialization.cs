using Common.Extensions;
using Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static Common.Reflection.ReflectionUtil;

namespace Common.Reflection;

public static class ReflectionSerialization
{
    private const string MISSING_TYPE = "MISSING_TYPE";
    private static readonly Assembly SystemAssembly = typeof(int).Assembly;
    private static readonly Dictionary<string, string> _typeShorthands = new()
    {
        { "object", "System.Object" },
        { "Object", "System.Object" },
        { "byte", "System.Byte" },
        { "Byte", "System.Byte" },
        { "short", "System.Int16" },
        { "Int16", "System.Int16" },
        { "ushort", "System.UInt16" },
        { "UInt16", "System.UInt16" },
        { "int", "System.Int32" },
        { "Int32", "System.Int32" },
        { "uint", "System.UInt32" },
        { "UInt32", "System.UInt32" },
        { "long", "System.Int64" },
        { "Int64", "System.Int64" },
        { "ulong", "System.UInt64" },
        { "UInt64", "System.UInt64" },
        { "float", "System.Single" },
        { "Single", "System.Single" },
        { "double", "System.Double" },
        { "Double", "System.Double" },
        { "decimal", "System.Decimal" },
        { "Decimal", "System.Decimal" },
        { "string", "System.String" },
        { "String", "System.String" },
        { "boolean", "System.Boolean" },
        { "Boolean", "System.Boolean" },
        { "DateTime", "System.DateTime" },
    };
    private static readonly Dictionary<string, string> _toTypeShorthands = new()
    {
        { "System.Object", "object" },
        { "System.Byte", "byte" },
        { "System.Int16", "short" },
        { "System.UInt16", "ushort" },
        { "System.Int32", "int" },
        { "System.UInt32", "uint" },
        { "System.Int64", "long" },
        { "System.UInt64", "ulong" },
        { "System.Single", "float" },
        { "System.Double", "double" },
        { "System.Decimal", "decimal" },
        { "System.String", "string" },
        { "System.Boolean", "boolean" },
    };
    private const string PARAMETER_SEPARATOR = "_;_";

    public static string SerializeMethod(MethodInfo method)
    {
        MethodInfo? genericMethod = null;
        if (method.IsGenericMethod || method.ContainsGenericParameters)
            try
            {
                genericMethod = method.GetGenericMethodDefinition();
            }
            catch (Exception) { }
        var sb = new StringBuilder();
        var fullType = SerializeType(method.DeclaringType ?? throw new ArgumentException($"No declaring type for methodInfo '{method.Name}'."));
        sb.Append(fullType);
        sb.Append(':');
        sb.Append(method.Name);
        sb.Append('(');
        var parameters = method.GetParameters();
        var genericParameters = genericMethod?.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            sb.Append(SerializeParameter(method, parameters[i], genericParameters?[i]));
            if (i < parameters.Length - 1) sb.Append(PARAMETER_SEPARATOR);
        }
        sb.Append(')');
        return sb.ToString();
    }

    public static string SerializeParameter(MethodInfo method, ParameterInfo parameter, ParameterInfo? genericParameter = null)
    {
        var sb = new StringBuilder();
        var isGeneric = genericParameter?.ParameterType.IsGenericParameter ?? false;
        if (isGeneric) sb.Append('-');
        sb.Append(isGeneric || parameter.ParameterType.Assembly == method.DeclaringType?.Assembly || parameter.ParameterType.Assembly == SystemAssembly
            ? parameter.ParameterType : SerializeType(parameter.ParameterType));
        return sb.ToString();
    }

    public static string SerializeProperty(PropertyInfo property)
    {
        var sb = new StringBuilder();
        var fullType = SerializeType(property.DeclaringType ?? throw new ArgumentException($"No declaring type for property '{property.Name}'."));
        sb.Append(fullType);
        sb.Append(':');
        sb.Append(property.Name);
        return sb.ToString();
    }

    public static string SerializeField(FieldInfo field)
    {
        var sb = new StringBuilder();
        var fullType = SerializeType(field.DeclaringType ?? throw new ArgumentException($"No declaring type for field '{field.Name}'."));
        sb.Append(fullType);
        sb.Append(':');
        sb.Append(field.Name);
        return sb.ToString();
    }

    public static string SerializeMethod<T>(string methodName) => SerializeMethod(GetMethodInfo<T>(methodName));
    public static string SerializeMethod<T, T1>(string methodName) => SerializeMethod(GetMethodInfo<T, T1>(methodName));
    public static string SerializeMethod<T, T1, T2>(string methodName) => SerializeMethod(GetMethodInfo<T, T1, T2>(methodName));
    public static string SerializeMethod<T, T1, T2, T3>(string methodName) => SerializeMethod(GetMethodInfo<T, T1, T2, T3>(methodName));
    public static string SerializeMethod<T, T1, T2, T3, T4>(string methodName) => SerializeMethod(GetMethodInfo<T, T1, T2, T3, T4>(methodName));

    public static string SerializeMethod<T1, TOut>(Expression<Func<T1, TOut>> expression) => SerializeMethod(GetMethodInfo(expression));
    public static string SerializeMethod<T1, T2, TOut>(Expression<Func<T1, T2, TOut>> expression) => SerializeMethod(GetMethodInfo(expression));
    public static string SerializeMethod<T1, T2, T3, TOut>(Expression<Func<T1, T2, T3, TOut>> expression) => SerializeMethod(GetMethodInfo(expression));
    public static string SerializeMethod<T1, T2, T3, T4, TOut>(Expression<Func<T1, T2, T3, T4, TOut>> expression) => SerializeMethod(GetMethodInfo(expression));
    public static string SerializeMethod<T1, T2, T3, T4, T5, TOut>(Expression<Func<T1, T2, T3, T4, T5, TOut>> expression) => SerializeMethod(GetMethodInfo(expression));
    public static string SerializeProperty<T>(string propertyName) => SerializeProperty(typeof(T).GetProperty(propertyName) ?? throw new ArgumentException($"Can't find property '{propertyName}'."));

    public static MethodInfo? DeserializeMethod(string src)
    {
        var args = StringUtil.GetInside(src, "(", ")").Split(PARAMETER_SEPARATOR).Where(x => !string.IsNullOrEmpty(x)).Select(x => DeserializeType(x)).ToArray();
        var fullNameWithMethod = src.Split("(")[0];
        var splittedFullName = fullNameWithMethod.Split(':');
        var fullNameWithAssembly = splittedFullName[0];
        var type = Type.GetType(fullNameWithAssembly);
        if (type == null) throw new ArgumentException($"Could not find type: '{fullNameWithAssembly}' for src '{src}'");
        var methodName = splittedFullName[1];
        try
        {
            return type.GetMethod(methodName);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static MethodInfo DeserializeMethod(Type type, string src)
    {
        var args = StringUtil.GetInside(src, "(", ")").Split(PARAMETER_SEPARATOR).Select(x => DeserializeParameter(x, type.Assembly)).ToArray();
        var fullNameWithMethod = src.Split("(")[0];
        var splittedFullName = fullNameWithMethod.Split(':');
        var fullName = splittedFullName[0];
        var methodName = splittedFullName[1];
        var parameterCount = args.Length;
        var genericArgumentCount = args.Count(x => x.IsGeneric);
        return GetGenericMethod(type, methodName, parameterCount, genericArgumentCount, args.Select(x => x.Type).ToArray())!;
    }

    public static MethodInfo DeserializeMethod<T>(string src) => DeserializeMethod(typeof(T), src);

    public static (Type? Type, bool IsGeneric) DeserializeParameter(string src, Assembly? assembly = null) => (src == "-" ? null : DeserializeType(src, assembly), src.StartsWith("-"));

    public static PropertyInfo DeserializeProperty(string src)
    {
        var fullNameWithPropery = src.Split('(')[0];
        var splittedFullName = fullNameWithPropery.Split(':');
        var fullNameWithAssembly = splittedFullName[0];
        var type = Type.GetType(fullNameWithAssembly);
        if (type == null) throw new ArgumentException($"Could not find type: '{fullNameWithAssembly}' for src '{src}'");
        var properyName = splittedFullName[1];
        return type.GetProperty(properyName)!;
    }

    public static FieldInfo DeserializeField(string src)
    {
        var fullNameWithField = src.Split("(")[0];
        var splittedFullName = fullNameWithField.Split(':');
        var fullNameWithAssembly = splittedFullName[0];
        var type = Type.GetType(fullNameWithAssembly);
        if (type == null) throw new ArgumentException($"Could not find type: '{fullNameWithAssembly}' for src '{src}'");
        var fieldName = splittedFullName[1];
        return type.GetField(fieldName)!;
    }

    public static string SerializeType(Type? type) => type == null ? MISSING_TYPE : $"{type.FullName}, {type.Assembly.FullName}";
    public static string SerializeType<T>() => SerializeType(typeof(T));

    private static IDictionary<string, Type> _deserializeTypeCache = new Dictionary<string, Type>();
    public static Type DeserializeType(string valueType, Assembly? assemblyIn = null)
    {
        if (_deserializeTypeCache.ContainsKey(valueType)) return _deserializeTypeCache[valueType];
        if (string.IsNullOrEmpty(valueType) || valueType == MISSING_TYPE) return null!;
        if (valueType == "void") return typeof(void);
        var type = Type.GetType(valueType);
        if (type != null) return type;

        assemblyIn ??= GetAssemblyFromTypePath(valueType);
        if (assemblyIn != null) return assemblyIn.GetType(valueType)!;

        try
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //To speed things up, we check first in the already loaded assemblies.
            foreach (var assembly in assemblies)
            {
                type = assembly.GetType(valueType);
                if (type != null)
                    break;
            }
            if (type != null)
            {
                _deserializeTypeCache[valueType] = type;
                return type;
            }

            var loadedAssemblies = assemblies.ToList();

            foreach (var loadedAssembly in assemblies)
            {
                foreach (var referencedAssemblyName in loadedAssembly.GetReferencedAssemblies())
                {
                    var found = loadedAssemblies.All(x => x.GetName() != referencedAssemblyName);

                    if (!found)
                    {
                        try
                        {
                            var referencedAssembly = Assembly.Load(referencedAssemblyName);
                            type = referencedAssembly.GetType(valueType);
                            if (type != null)
                                break;
                            loadedAssemblies.Add(referencedAssembly);
                        }
                        catch
                        {
                            //We will ignore this, because the Type might still be in one of the other Assemblies.
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            throw;
        }

        _deserializeTypeCache[valueType] = type!;
        return type!;
    }

    public static Type DeserializeTypeLookAtShortNames(string type) => DeserializeType(GetShortHandName(type));
    public static bool IsShortHandName(string typeName) => _typeShorthands.ContainsKey(typeName);
    public static string GetShortHandName(string typeName) => _typeShorthands.ContainsKey(typeName) ? _typeShorthands[typeName] : typeName;
    public static string GetToShortHandName(string typeName) => _toTypeShorthands.ContainsKey(GetShortHandName(typeName)) ? _toTypeShorthands[GetShortHandName(typeName)] : typeName;

    private static IDictionary<string, Assembly> _typeToAssemblyCache = new Dictionary<string, Assembly>();
    public static Assembly DeserializeAssembly(string name)
    {
        var isCached = _typeToAssemblyCache.ContainsKey(name);
        var result = isCached ? _typeToAssemblyCache[name] : GetAssembly(name)!;
        if (!isCached) _typeToAssemblyCache[name] = result;
        return result;
    }

    public static string SerializeAssembly(Assembly assembly) => assembly.GetName().FullName;
    private static string AddAssemblyToTypePath(string pathWithOrWithoutAssembly, Assembly? assembly = null)
        => assembly == null || TypePathContainsAnAssembly(pathWithOrWithoutAssembly) ? pathWithOrWithoutAssembly : $"{pathWithOrWithoutAssembly}, {SerializeAssembly(assembly!)}";
    private static bool TypePathContainsAnAssembly(string pathWithOrWithoutAssembly) => pathWithOrWithoutAssembly.Contains(' ');
    private static Assembly? GetAssemblyFromTypePath(string pathWithOrWithoutAssembly) => TypePathContainsAnAssembly(pathWithOrWithoutAssembly) ? DeserializeAssembly(pathWithOrWithoutAssembly.Split(' ')[1]) : null;
}
