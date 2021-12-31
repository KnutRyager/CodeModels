using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Compilation;
using CodeAnalyzation.Models;
using Common.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation
{
    public static class SyntaxNodeExtensions
    {
        private static readonly IDictionary<string?, CompilationContext> contexts = new ConcurrentDictionary<string?, CompilationContext>();
        private static readonly IDictionary<SyntaxTree, CompilationContext> treeContexts = new ConcurrentDictionary<SyntaxTree, CompilationContext>();
        private static string? lastKey;
        public static void SetCompilation(CSharpCompilation compilation, IEnumerable<SyntaxTree> trees, string? key = null)
        {
            foreach (var tree in trees)
            {
                GetContext(tree).SetCompilation(compilation, trees);
                GetContext(key).SetCompilation(compilation, trees);
            }
            GetContext(key).SetCompilation(compilation, trees);
        }

        public static void SetSemanticModel(SyntaxTree tree, string? key = null) => GetContext(tree, key).SetSemanticModel(tree);
        public static void SetSemanticModel(int treeIndex, string? key = null) => GetContext(key).SetSemanticModel(treeIndex);
        public static SemanticModel GetSemanticModel(SyntaxTree? tree = null, string? key = null) => (tree != null ? GetContext(tree, key) : GetContext(key)).GetSemanticModel(tree);

        public static bool HasModifier(this MemberDeclarationSyntax node, SyntaxKind modifier) => node.Modifiers.Any(modifier);
        public static bool IsStatic(this MemberDeclarationSyntax node) => node.HasModifier(SyntaxKind.StaticKeyword);
        public static bool IsNonStatic(this MemberDeclarationSyntax node) => !node.IsStatic();
        public static bool IsPublic(this MemberDeclarationSyntax node) => node is NamespaceDeclarationSyntax || node.HasModifier(SyntaxKind.PublicKeyword);
        public static bool IsPrivate(this MemberDeclarationSyntax node) => node.HasModifier(SyntaxKind.PrivateKeyword);
        public static bool IsProtected(this MemberDeclarationSyntax node) => node.HasModifier(SyntaxKind.ProtectedKeyword);
        
        public static IEnumerable<NamespaceDeclarationSyntax> GetNamespaces(this CSharpSyntaxNode node)
            => from @namespace in node.DescendantNodes().OfType<NamespaceDeclarationSyntax>() select @namespace;

        public static IEnumerable<ClassDeclarationSyntax> GetClasses(this CSharpSyntaxNode node)
            => from classDeclaration in node.DescendantNodes().OfType<ClassDeclarationSyntax>() select classDeclaration;

        public static IEnumerable<ClassDeclarationSyntax> GetStaticClasses(this CSharpSyntaxNode node)
            => node.GetClasses().Where(IsStatic);

        public static IEnumerable<ClassDeclarationSyntax> GetNonStaticClasses(this CSharpSyntaxNode node)
            => node.GetClasses().Where(IsNonStatic);

        public static IEnumerable<MethodDeclarationSyntax> GetMethods(this CSharpSyntaxNode node)
            => from methodDeclaration in node.DescendantNodes().OfType<MethodDeclarationSyntax>() select methodDeclaration;

        public static SyntaxNode GetVisit(this SyntaxNode node, CSharpSyntaxRewriter visitor) => visitor.Visit(node);
        public static SyntaxNode GetVisit<T>(this SyntaxNode node) where T : CSharpSyntaxRewriter, new()
            => node.GetVisit(new T());

        public static IEnumerable<MethodDeclarationSyntax> GetPublicMethods(this CSharpSyntaxNode node)
            => node.GetMethods().Where(IsPublic);

        public static string GetName(this MethodDeclarationSyntax method)
            => method.Identifier.ValueText;

        public static SeparatedSyntaxList<ParameterSyntax> GetParameters(this MethodDeclarationSyntax method)
            => method.ParameterList.Parameters;

        public static TypeSyntax GetReturn(this MethodDeclarationSyntax method)
            => method.ReturnType;

        public static IEnumerable<PropertyDeclarationSyntax> GetProperties(this CSharpSyntaxNode node)
            => node.DescendantNodes().OfType<PropertyDeclarationSyntax>();

        public static IEnumerable<FieldDeclarationSyntax> GetFields(this CSharpSyntaxNode node)
            => node.DescendantNodes().OfType<FieldDeclarationSyntax>();

        public static IEnumerable<MemberDeclarationSyntax> GetMembers(this CSharpSyntaxNode node)
            => node.DescendantNodes().OfType<MemberDeclarationSyntax>();

        public static IEnumerable<VariableDeclarationSyntax> GetVariables(this CSharpSyntaxNode node)
            => node.DescendantNodes().OfType<VariableDeclarationSyntax>();

        public static List<ISymbol> GetMemberSymbols(this CSharpSyntaxNode node, SemanticModel model)
            => node.GetMembers().ToList<SyntaxNode>().Concat(node.GetFields().SelectMany(y => y.Declaration.Variables)).Select(x => model.GetDeclaredSymbol(x)).Where(x => x is not null).ToList()!;

        // public static IEnumerable<ClassDeclarationSyntax> GetDerivedClasses(this ClassDeclarationSyntax node)
        // {
        //     var classDecSynList = classDecSynList.Where(x =>
        //(((IdentifierNameSyntax(x.BaseList.Types.FirstOrDefault()))
        //       .Identifier.ValueText == "Base"));
        //     return node.DescendantNodes().OfType<VariableDeclarationSyntax>();
        // }

        //public class BaseClassRewriter : CSharpSyntaxRewriter
        //{
        //    private readonly SemanticModel _model;

        //    public BaseClassRewriter(SemanticModel model)
        //    {
        //        _model = model;
        //    }

        //    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        //    {
        //        var symbol = _model.GetDeclaredSymbol(node);
        //        if (InheritsFrom<SyntaxNode>(symbol))
        //        {
        //            // hit!
        //        }
        //    }

        //    private bool InheritsFrom<T>(INamedTypeSymbol symbol)
        //    {
        //        while (true)
        //        {
        //            if (symbol.ToString() == typeof(T).FullName)
        //            {
        //                return true;
        //            }
        //            if (symbol.BaseType != null)
        //            {
        //                symbol = symbol.BaseType;
        //                continue;
        //            }
        //            break;
        //        }
        //        return false;
        //    }
        //}

        public static IEnumerable<FieldDeclarationSyntax> GetPublicStaticFields(this CSharpSyntaxNode node)
        => node.DescendantNodes().OfType<FieldDeclarationSyntax>().Where(IsPublic);

        public static ISymbol GetType(this CSharpSyntaxNode node, string? key = null) => GetContext(key).SemanticModel.GetSymbolInfo(node).Symbol!;

        public static SyntaxNode GetContentRoot(this SyntaxNode node) => node switch
        {
            PropertyDeclarationSyntax property => property.ExpressionBody ?? (SyntaxNode?)property.AccessorList!,
            FieldDeclarationSyntax field => field.Declaration,
            MethodDeclarationSyntax method => method.Body ?? (SyntaxNode?)method.ExpressionBody!,
            ArrowExpressionClauseSyntax arrowClause => arrowClause.Expression!,
            VariableDeclaratorSyntax variable => variable.Initializer!,
            _ => throw new ArgumentException($"Can't find content root of node '{node}'")
        };

        private static CompilationContext GetContext(SyntaxTree tree, string? key = null)
        {
            if (key != null) return GetContext(key);
            var context = treeContexts.ContainsKey(tree) ? treeContexts[tree] : new CompilationContext();
            treeContexts[tree] = context;
            return context;
        }

        private static CompilationContext GetContext(string? key = null)
        {
            key ??= lastKey;
            var context = contexts.ContainsKey(key) ? contexts[key] : new CompilationContext();
            contexts[key] = context;
            lastKey = key;
            return context;
        }
    }
}