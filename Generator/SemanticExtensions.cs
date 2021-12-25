using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.SyntaxNodeExtensions;

namespace CodeAnalyzation
{
    public static class SemanticExtensions
    {
        public static IDictionary<ISymbol, List<SymbolDependencies>> GetDependencies(this IEnumerable<ClassDeclarationSyntax> classes, SemanticModel model)
        {
            var modelSymbols = classes.Select(x => (model.GetDeclaredSymbol(x)!)).ToList();
            return classes.ToDictionary(x => model.GetDeclaredSymbol(x) ?? throw new ArgumentException($"No declared symbol for '{x}'."),
                x => GetDependencies(x, modelSymbols, model),
                SymbolEqualityComparer.Default)!;
        }

        public static List<SymbolDependencies> GetDependencies(ClassDeclarationSyntax @class, IEnumerable<ISymbol> modelSymbols, SemanticModel model)
            => @class.GetMemberSymbols(model).Select(x => x.GetDependencies(modelSymbols)).ToList();

        public static SymbolDependencies GetDependencies(this ISymbol symbol, IEnumerable<ISymbol> modelSymbols)
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

        public static List<Tree<PathPiece>> GetPathPieces(List<SyntaxNodeDependencies> dependencies, ISymbol startClass)
        {
            var pathPieces = dependencies.Select(x => new PathPiece(x.ContainingClass, x.Type, x.Symbol, x.Name)).Distinct().ToList();
            var rootPieces = pathPieces.Where(x => SymbolEqualityComparer.Default.Equals(x.From, startClass)).ToList();
            var trees = rootPieces.Select(x => new Tree<PathPiece>(x)).ToList();

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

        private static void CollectPathPieces(IEnumerable<PathPiece> allPieces, Tree<PathPiece> tree, HashSet<ISymbol> visitedClasses)
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
        public record SymbolDependencies(ISymbol Symbol, List<Tree<PathPiece>> Dependencies);
        public record PathPiece(ISymbol From, ISymbol To, ISymbol? Property, string Name);
    }
}