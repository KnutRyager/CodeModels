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
        return ReflectionUtil.GetMethodInfo(GetContainingType(symbol), symbol.Name, parameters);
    }

    public static Assembly GetAssembly(ISymbol symbol) => ReflectionSerialization.DeserializeAssembly(symbol.ContainingAssembly.ToString());
    public static Type GetContainingType(ISymbol symbol) 
        => symbol.ContainingType is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.Name is "Program" 
        ? null!
        :GetType(symbol.ContainingType);
    public static Type GetType(ITypeSymbol symbol) => GetType(symbol.ToString(), symbol);
    public static Type GetType(IMethodSymbol symbol) => GetType(symbol.ToString(), symbol);
    public static Type GetType(INamedTypeSymbol symbol) => GetType(symbol.ToString(), symbol);
    public static Type GetType(IArgumentOperation symbol) => GetType(symbol.Parameter);
    public static Type GetType(IParameterSymbol symbol) => GetType(symbol.ToString(), symbol);

    // https://docs.microsoft.com/en-us/dotnet/api/system.type.gettype?view=net-6.0
    // Not loading? https://jeremybytes.blogspot.com/2020/01/using-typegettype-with-net-core.html
    public static Type GetType(string name, ISymbol symbol)
    {
        //var fuuyuk = new ConcurrentDictionary<int, string>();
        //var fakka = fuuyuk.GetType().Assembly;
        //var aa = GetAssembly(symbol);
        //var ts = aa.GetTypes();
        //var t = aa.GetType(name);
        //var ts2 = ts.Where(x => x.FullName.Contains("ConcurrentDictionary")).ToArray();
        //var ts3 = fakka.GetTypes();
        //var t3 = fakka.GetType(name);
        //var ts4 = ts3.Where(x => x.FullName.Contains("ConcurrentDictionary")).ToArray();
        return TryGetType(name)
            ?? GetAssembly(symbol).GetTypes().FirstOrDefault(x => x.Name[0..(x.Name.Contains("`") ? x.Name.IndexOf("`") : x.Name.Length)] == name[0..(name.Contains("`") ? name.IndexOf("`") : name.Length)])
?? throw new Exception($"Type not found: '{ReflectionSerialization.GetShortHandName(ReflectionSerialization.NormalizeType(name.Replace("?", "")))}'.");
    }
    public static Type? TryGetType(string name) => Type.GetType(ReflectionSerialization.GetShortHandName(ReflectionSerialization.NormalizeType(name.Replace("?", ""))));

    public static Microsoft.CodeAnalysis.TypeInfo GetTypeInfo(SyntaxNode node, SemanticModel model) => model.GetTypeInfo(node);
    public static Type? GetType(SyntaxNode node, SemanticModel model) => GetType(GetTypeInfo(node, model).Type);
}
