using System;
using System.Linq;
using System.Reflection;
using Common.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace CodeAnalyzation.Reflection;

public static class SemanticReflection
{
    public static MemberInfo GetMemberInfo(ISymbol symbol) => symbol.OriginalDefinition switch
    {
        IFieldSymbol field => GetField(field),
        IPropertySymbol property => GetProperty(property),
        IMethodSymbol method => GetMethod(method),
        _ => throw new NotImplementedException($"Not implemented: '{symbol}'.")
    };

    public static FieldInfo GetField(IFieldSymbol symbol) => GetContainingType(symbol).GetField(symbol.Name);
    public static PropertyInfo GetProperty(IPropertySymbol symbol) => GetContainingType(symbol).GetProperty(symbol.Name);
    public static ConstructorInfo GetConstructor(IObjectCreationOperation symbol)
    {
        var methodSymbol = symbol.Constructor;
        var type = GetType(symbol.Type);
        var parameters = methodSymbol.Parameters.Select(GetType).ToArray();
        var constructor = type.GetConstructor(parameters);
        return constructor;
    }

    public static MethodInfo? GetMethod(IMethodSymbol symbol)
    {
        var parameters = symbol.Parameters.Select(x => GetType(x.Type)).ToArray();
        return ReflectionUtil.GetMethodInfo(GetContainingType(symbol),
            symbol.Name,
            symbol.MethodKind is MethodKind.ReducedExtension
                ? new Type[] { GetType(symbol.ReceiverType!) }.Concat(parameters).ToArray()
                : parameters);
    }

    public static Assembly GetAssembly(ISymbol symbol) => ReflectionSerialization.DeserializeAssembly(symbol.ContainingAssembly.ToString());
    public static Type GetContainingType(ISymbol symbol)
        => symbol.ContainingType is INamedTypeSymbol { Name: "Program" } namedTypeSymbol
        ? null!
        : GetType(symbol.ContainingType);
    public static Type GetType(ITypeSymbol symbol) => GetType(symbol.ToString(), symbol);
    public static Type GetType(IMethodSymbol symbol) => GetType(symbol.ToString(), symbol);
    public static Type GetType(INamedTypeSymbol symbol) => GetType(symbol.ToString(), symbol);
    public static Type GetType(IArgumentOperation symbol) => GetType(symbol.Parameter);
    public static Type GetType(IParameterSymbol symbol) => GetType(symbol.ToString(), symbol);

    // https://docs.microsoft.com/en-us/dotnet/api/system.type.gettype?view=net-6.0
    // Not loading? https://jeremybytes.blogspot.com/2020/01/using-typegettype-with-net-core.html
    public static Type GetType(string name, ISymbol symbol)
    {
        //var aa = GetAssembly(symbol);
        //var ts = aa.GetTypes();
        //var t = aa.GetType(name);
        //var ts2 = ts.Where(x => x.FullName.Contains("Console")).ToArray();
        var trimmedName = TrimGenericTypeName(name);
        return TryGetType(name)
            ?? GetAssembly(symbol).GetTypes().FirstOrDefault(x => ReplacePlusInPath(TrimGenericTypeName(x.Name)) == trimmedName || ReplacePlusInPath(TrimGenericTypeName(x.FullName)) == trimmedName)
            ?? throw new Exception($"Type not found: '{ReflectionSerialization.GetShortHandName(ReflectionSerialization.NormalizeType(name.Replace("?", "")))}'.");
    }

    private static string TrimGenericTypeName(string name)
        => name.Contains("`") ? name[0..name.IndexOf("`")] : name;

    private static string ReplacePlusInPath(string name)
        => name.Contains("+") ? name.Replace("+", ".") : name;

    public static Type? TryGetType(string name) => Type.GetType(ReflectionSerialization.GetShortHandName(ReflectionSerialization.NormalizeType(name.Replace("?", ""))));

    public static Microsoft.CodeAnalysis.TypeInfo GetTypeInfo(SyntaxNode node, SemanticModel model) => model.GetTypeInfo(node);
    public static Type? GetType(SyntaxNode node, SemanticModel model) => GetType(GetTypeInfo(node, model).Type);
}
