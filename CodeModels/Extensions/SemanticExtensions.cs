using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Extensions.SyntaxNodeExtensions;

namespace CodeModels.Extensions;

public static class SemanticExtensions
{
    public static IDictionary<ISymbol, List<MemberDependencies>> GetDirectDependencies(this IEnumerable<ClassDeclarationSyntax> classes, SemanticModel model)
    {
        var modelSymbols = classes.Select(x => model.GetDeclaredSymbol(x)!).ToList();
        return classes.ToDictionary(x => model.GetDeclaredSymbol(x) ?? throw new ArgumentException($"No declared symbol for '{x}'."),
            x => GetDependencies(x, modelSymbols, model),
            SymbolEqualityComparer.Default)!;
    }

    public static IDictionary<ISymbol, List<MemberDependencies>> GetFullDependencies(this IEnumerable<ClassDeclarationSyntax> classes, SemanticModel model)
    {
        var directDependencies = classes.GetDirectDependencies(model);
        var allMemberDirectDependencies = directDependencies.SelectMany(x => x.Value)
            .ToDictionary(x => x.Member, x => x.Dependencies, SymbolEqualityComparer.Default);
        var allMembers = allMemberDirectDependencies.Keys;
        var analysisStates = allMembers.ToDictionary(x => x, x => DependencyAnalysisState.INACTIVE, SymbolEqualityComparer.Default);
        var fullDependencies = new Dictionary<ISymbol, List<Tree<MemberDependency>>>(SymbolEqualityComparer.Default);
        foreach (var member in allMembers)
            GetFullDependencies(member, fullDependencies, allMemberDirectDependencies, model, analysisStates);

        return fullDependencies
            .GroupBy(x => x.Key.ContainingType, x => new MemberDependencies(x.Key, x.Value), SymbolEqualityComparer.Default)
            .ToDictionary(x => x.Key!, x => x.ToList(), SymbolEqualityComparer.Default);
    }

    public static void GetFullDependencies(ISymbol symbol,
        IDictionary<ISymbol, List<Tree<MemberDependency>>> fullDependencies,
        IDictionary<ISymbol, List<Tree<MemberDependency>>> allDirectDependencies,
        SemanticModel model,
        IDictionary<ISymbol, DependencyAnalysisState> states)
    {
        if (states[symbol] is not DependencyAnalysisState.INACTIVE) return;
        states[symbol] = DependencyAnalysisState.ACTIVE;
        var directDependenciesList = allDirectDependencies[symbol];
        foreach (var dependency in directDependenciesList)
        {
            foreach (var path in dependency.ToList())
            {
                GetFullDependencies(path.Property, fullDependencies, allDirectDependencies, model, states);
            }
        }

        var fullDependency = directDependenciesList.Select(x => new Tree<MemberDependency>(x)).ToList();
        var additoalTopDependencies = new List<Tree<MemberDependency>>();
        foreach (var dependency in fullDependency)
        {
            var additionalDependencies = new List<(Tree<MemberDependency> Tree, List<Tree<MemberDependency>> Entries)>();
            dependency.TraverseNodes(x =>
            {
                var propDependencies = fullDependencies[x.Data.Property];
                var pathToConnectTo = x.Data.Property.ContainingType;
                if (x.Parent is null)
                {
                    additoalTopDependencies.AddRange(propDependencies);
                }
                else
                {
                    additionalDependencies.Add((x, propDependencies));
                }
            });
            foreach (var entry in additionalDependencies)
            {
                entry.Tree.Parent!.AddRange(entry.Entries);
            }
        }
        fullDependency.AddRange(additoalTopDependencies);
        fullDependencies[symbol] = fullDependency;

        states[symbol] = DependencyAnalysisState.COMPLETE;
    }

    public static List<MemberDependencies> GetDependencies(ClassDeclarationSyntax @class, IEnumerable<ISymbol> modelSymbols, SemanticModel model)
        => @class.GetMemberSymbols(model).Select(x => x.GetDependencies(modelSymbols)).ToList();

    public static MemberDependencies GetDependencies(this ISymbol symbol, IEnumerable<ISymbol> modelSymbols)
    {
        var member = symbol.DeclaringSyntaxReferences.First().GetSyntax();
        var semanticModel = GetSemanticModel(tree: member.SyntaxTree);
        var dependencies = new List<SyntaxNodeDependencies>();
        var root = member.GetContentRoot();
        CollectDependencies(root, dependencies, modelSymbols);
        dependencies = dependencies.Distinct().ToList();
        var memberSymbol = semanticModel.GetDeclaredSymbol(member);
        var ffixed = GetPathPieces(dependencies, memberSymbol!.ContainingType);
        return new(memberSymbol, ffixed);
    }

    public static List<Tree<MemberDependency>> GetPathPieces(List<SyntaxNodeDependencies> dependencies, ISymbol startClass)
    {
        var pathPieces = dependencies.Select(x => new MemberDependency(x.ContainingClass, x.Type, x.Symbol, x.Name)).Distinct().ToList();
        var rootPieces = pathPieces.Where(x => SymbolEqualityComparer.Default.Equals(x.From, startClass)).ToList();
        var trees = rootPieces.Select(x => new Tree<MemberDependency>(x)).ToList();

        foreach (var tree in trees)
        {
            var visitedClasses = new HashSet<ISymbol>(SymbolEqualityComparer.Default) { startClass };
            CollectPathPieces(pathPieces, tree, visitedClasses);
        }
        return trees;
    }

    private static List<SyntaxNodeDependencies> CollectDependencies(SyntaxNode node, List<SyntaxNodeDependencies> dependencies, IEnumerable<ISymbol> modelSymbols)
    {
        var semanticModel = GetSemanticModel(tree: node.SyntaxTree);
        var symbol = semanticModel.GetSymbolInfo(node).Symbol;
        var type = semanticModel.GetOperation(node)?.Type;
        var parentType = node.Parent == null ? null : semanticModel.GetOperation(node.Parent)?.Type;

        if ((symbol is IPropertySymbol || symbol is IMethodSymbol || symbol is IFieldSymbol)
            && modelSymbols.Contains(symbol.ContainingSymbol, SymbolEqualityComparer.Default))
        {
            dependencies.Add(new(node, symbol, symbol.ContainingSymbol!, type ?? parentType!, symbol.MetadataName));
        }

        foreach (var descendant in node.DescendantNodes())
        {
            CollectDependencies(descendant, dependencies, modelSymbols);
        }

        return dependencies;
    }

    private static void CollectPathPieces(IEnumerable<MemberDependency> allPieces, Tree<MemberDependency> tree, HashSet<ISymbol> visitedClasses)
    {
        var startClass = tree.Data.To;
        if (visitedClasses.Contains(startClass)) return;
        visitedClasses.Add(startClass);
        var pathPieces = allPieces.Where(x => SymbolEqualityComparer.Default.Equals(x.From, startClass)).ToList();
        foreach (var piece in pathPieces)
        {
            var childTree = tree.Add(piece);
            tree.Add(childTree);
            CollectPathPieces(allPieces, childTree, visitedClasses);
        }
    }

    public record SyntaxNodeDependencies(SyntaxNode Node, ISymbol Symbol, ISymbol ContainingClass, ITypeSymbol Type, string Name);
    public record MemberDependencies(ISymbol Member, List<Tree<MemberDependency>> Dependencies);
    public record MemberDependency(ISymbol From, ISymbol To, ISymbol Property, string Name);

    public enum DependencyAnalysisState
    {
        INACTIVE, ACTIVE, COMPLETE, FAILED
    }
}
