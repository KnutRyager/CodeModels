using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Reflection;

public static class SymbolUtils
{
    public static bool IsNewDefined(ISymbol symbol) => symbol.ContainingAssembly?.Name.StartsWith("MyCompilation") ?? false;
    public static bool IsNewDefined(IOperation symbol) => IsNewDefined(symbol.Type ?? throw new NotImplementedException());
    public static ISymbol? GetSymbol(SyntaxNode node, SemanticModel model) => model?.GetSymbolInfo(node).Symbol;
    public static INamedTypeSymbol? GetType(SyntaxNode node, SemanticModel model)
    {
        var declaredSymbol = GetDeclaredSymbol(node, model);
        return declaredSymbol?.ContainingType;
    }

    public static ISymbol? GetDeclaredSymbol(SyntaxNode node, SemanticModel model) => model?.GetDeclaredSymbol(node);

    public static ISymbol GetMember(ISymbol symbol) => symbol switch
    {
        IFieldSymbol field => field,
        _ => symbol
    };

    public static IAssemblySymbol? GetAssembly(CompilationUnitSyntax node, SemanticModel model)
        => model.GetDeclaredSymbol(node)?.ContainingAssembly ?? (node.Members.Count > 0 ? GetAssembly(node.Members[0], model) : null);

    public static IAssemblySymbol GetAssembly(MemberDeclarationSyntax node, SemanticModel model)
        => model.GetDeclaredSymbol(node)?.ContainingAssembly ?? throw new NotImplementedException();

    public static ISymbol? GetDeclaration(SyntaxNode node, SemanticModel model) => node switch
    {
        FieldDeclarationSyntax field => GetDeclaration(field, model),
        PropertyDeclarationSyntax property => GetDeclaration(property, model),
        MethodDeclarationSyntax method => GetDeclaration(method, model),
        ConstructorDeclarationSyntax constructor => GetDeclaration(constructor, model),
        ClassDeclarationSyntax @class => GetDeclaredSymbol(@class, model),
        RecordDeclarationSyntax recordDeclaration => GetDeclaredSymbol(recordDeclaration, model),
        StructDeclarationSyntax structDeclaration => GetDeclaredSymbol(structDeclaration, model),
        InterfaceDeclarationSyntax interfaceDeclaration => GetDeclaredSymbol(interfaceDeclaration, model),
        IdentifierNameSyntax identifier => GetDeclaredSymbol(identifier, model) ?? GetSymbol(identifier, model),
        QualifiedNameSyntax qualifiedName => GetDeclaredSymbol(qualifiedName, model) ?? GetSymbol(qualifiedName, model),
        ObjectCreationExpressionSyntax objectCreation => GetDeclaration(objectCreation, model),
        LocalFunctionStatementSyntax localFunctionStatement => GetDeclaredSymbol(localFunctionStatement, model),
        EnumDeclarationSyntax @enum => GetDeclaredSymbol(@enum, model),
        _ => throw new NotImplementedException()
    };

    public static ISymbol? GetDeclaration(FieldDeclarationSyntax node, SemanticModel model)
        => GetDeclaredSymbol(node.Declaration.Variables.FirstOrDefault() ?? throw new NotImplementedException(), model);
    public static ISymbol? GetDeclaration(PropertyDeclarationSyntax node, SemanticModel model) => GetDeclaredSymbol(node, model);
    public static ISymbol? GetDeclaration(MethodDeclarationSyntax node, SemanticModel model) => GetDeclaredSymbol(node, model);
    public static ISymbol? GetDeclaration(ConstructorDeclarationSyntax node, SemanticModel model) => GetDeclaredSymbol(node, model);
    public static ISymbol? GetDeclaration(ObjectCreationExpressionSyntax node, SemanticModel model) => GetDeclaration(node.Type, model);
}