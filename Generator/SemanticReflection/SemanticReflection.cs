using System;
using System.Reflection;
using System.Text;
using System.Linq;
using Common.Extensions;
using Common.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using CodeAnalyzation.Models;

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

    public static MethodInfo GetMethod(IMethodSymbol symbol)
    {
        var parameters = symbol.Parameters.Select(x => GetType(x.Type)).ToArray();
        return ReflectionUtil.GetMethodInfo(GetContainingType(symbol), symbol.Name, parameters);
    }

    public static Type GetContainingType(ISymbol symbol) => GetType(symbol.ContainingType);
    public static Type GetType(ITypeSymbol symbol) => GetType(symbol.ToString());
    public static Type GetType(IMethodSymbol symbol) => GetType(symbol.ToString());
    public static Type GetType(INamedTypeSymbol symbol) => GetType(symbol.ToString());
    public static Type GetType(IArgumentOperation symbol) => GetType(symbol.Parameter);
    public static Type GetType(IParameterSymbol symbol) => GetType(symbol.ToString());
    // https://docs.microsoft.com/en-us/dotnet/api/system.type.gettype?view=net-6.0
    public static Type GetType(string name) => Type.GetType(ReflectionSerialization.GetShortHandName(ReflectionSerialization.NormalizeType(name.Replace("?", ""))));
    public static Microsoft.CodeAnalysis.TypeInfo GetTypeInfo(SyntaxNode node, SemanticModel model) => model.GetTypeInfo(node);
    public static Type? GetType(SyntaxNode node, SemanticModel model) => GetType(GetTypeInfo(node, model).Type);
}
