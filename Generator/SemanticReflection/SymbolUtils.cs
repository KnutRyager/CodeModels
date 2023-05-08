﻿using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Reflection;

public static class SymbolUtils
{
    public static bool IsNewDefined(ISymbol symbol) => symbol.ContainingAssembly?.Name.StartsWith("MyCompilation") ?? false;
    public static bool IsNewDefined(IOperation symbol) => IsNewDefined(symbol.Type);
    public static ISymbol? GetSymbol(SyntaxNode node, SemanticModel? model = null) => model?.GetSymbolInfo(node).Symbol;
    public static INamedTypeSymbol? GetType(SyntaxNode node, SemanticModel? model = null)
    {
        var declaredSymbol = GetDeclaredSymbol(node, model);
        return declaredSymbol.ContainingType;
    }

    public static ISymbol? GetDeclaredSymbol(SyntaxNode node, SemanticModel? model = null) => model?.GetDeclaredSymbol(node);

    public static ISymbol GetMember(ISymbol symbol) => symbol switch
    {
        IFieldSymbol field => field,
        _ => symbol
    };

    public static ISymbol? GetDeclaration(SyntaxNode node, SemanticModel? model = null) => node switch
    {
        FieldDeclarationSyntax field => GetDeclaration(field, model),
        PropertyDeclarationSyntax property => GetDeclaration(property, model),
        MethodDeclarationSyntax method => GetDeclaration(method, model),
        ConstructorDeclarationSyntax constructor => GetDeclaration(constructor, model),
        ClassDeclarationSyntax @class => GetDeclaredSymbol(@class, model),
        IdentifierNameSyntax identifier => GetDeclaredSymbol(identifier, model) ?? GetSymbol(identifier, model),
        QualifiedNameSyntax qualifiedName => GetDeclaredSymbol(qualifiedName, model) ?? GetSymbol(qualifiedName, model),
        ObjectCreationExpressionSyntax objectCreation => GetDeclaration(objectCreation, model),
        _ => throw new NotImplementedException()
    };

    public static ISymbol? GetDeclaration(FieldDeclarationSyntax node, SemanticModel? model = null)
        => GetDeclaredSymbol(node.Declaration.Variables.FirstOrDefault(), model);
    public static ISymbol? GetDeclaration(PropertyDeclarationSyntax node, SemanticModel? model = null) => GetDeclaredSymbol(node.Parent, model);
    public static ISymbol? GetDeclaration(MethodDeclarationSyntax node, SemanticModel? model = null) => GetDeclaredSymbol(node, model);
    public static ISymbol? GetDeclaration(ConstructorDeclarationSyntax node, SemanticModel? model = null) => GetDeclaredSymbol(node.Parent, model);
    public static ISymbol? GetDeclaration(ObjectCreationExpressionSyntax node, SemanticModel? model = null) => GetDeclaration(node.Type, model);
}