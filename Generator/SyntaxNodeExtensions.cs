using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation
{
    public static class SyntaxNodeExtensions
    {
        private static IDictionary<string?, CompilationContext> contexts = new ConcurrentDictionary<string?, CompilationContext>();
        private static IDictionary<SyntaxTree, CompilationContext> treeContexts = new ConcurrentDictionary<SyntaxTree, CompilationContext>();
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

        public static bool HasModifier(this MemberDeclarationSyntax root, SyntaxKind modifier) => root.Modifiers.Any(modifier);
        public static bool IsStatic(this MemberDeclarationSyntax root) => root.HasModifier(SyntaxKind.StaticKeyword);
        public static bool IsNonStatic(this MemberDeclarationSyntax root) => !root.IsStatic();
        public static bool IsPublic(this MemberDeclarationSyntax root) => root is NamespaceDeclarationSyntax || root.HasModifier(SyntaxKind.PublicKeyword);
        public static bool IsPrivate(this MemberDeclarationSyntax root) => root.HasModifier(SyntaxKind.PrivateKeyword);
        public static bool IsProtected(this MemberDeclarationSyntax root) => root.HasModifier(SyntaxKind.ProtectedKeyword);

        public static IEnumerable<NamespaceDeclarationSyntax> GetNamespaces(this CSharpSyntaxNode root)
            => from @namespace in root.DescendantNodes().OfType<NamespaceDeclarationSyntax>() select @namespace;

        public static IEnumerable<ClassDeclarationSyntax> GetClasses(this CSharpSyntaxNode root)
            => from classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>() select classDeclaration;

        public static IEnumerable<ClassDeclarationSyntax> GetStaticClasses(this CSharpSyntaxNode root)
            => root.GetClasses().Where(IsStatic);

        public static IEnumerable<ClassDeclarationSyntax> GetNonStaticClasses(this CSharpSyntaxNode root)
            => root.GetClasses().Where(IsNonStatic);

        public static IEnumerable<MethodDeclarationSyntax> GetMethods(this CSharpSyntaxNode root)
            => from methodDeclaration in root.DescendantNodes().OfType<MethodDeclarationSyntax>() select methodDeclaration;

        public static SyntaxNode GetVisit(this SyntaxNode root, CSharpSyntaxRewriter visitor) => visitor.Visit(root);
        public static SyntaxNode GetVisit<T>(this SyntaxNode root) where T : CSharpSyntaxRewriter, new()
            => root.GetVisit(new T());

        public static IEnumerable<MethodDeclarationSyntax> GetPublicMethods(this CSharpSyntaxNode root)
            => root.GetMethods().Where(IsPublic);

        public static string GetName(this MethodDeclarationSyntax method)
            => method.Identifier.ValueText;

        public static SeparatedSyntaxList<ParameterSyntax> GetParameters(this MethodDeclarationSyntax method)
            => method.ParameterList.Parameters;

        public static TypeSyntax GetReturn(this MethodDeclarationSyntax method)
            => method.ReturnType;

        public static Namespace GetModel(this NamespaceDeclarationSyntax @namespace)
            => new(new[] { @namespace.Name.ToString() });

        public static ClassModel GetModel(this ClassDeclarationSyntax @class, NamespaceDeclarationSyntax? @namespace)
            => @class.IsStatic() ? new StaticClass(@class.Identifier.ValueText,
                @namespace?.GetModel(),
                @class.GetMethods().Select(x => x.GetModel()))
            : new NonStaticClass(@class.Identifier.ValueText,
                @namespace?.GetModel(),
                @class.GetMethods().Select(x => x.GetModel()));

        public static Method GetModel(this MethodDeclarationSyntax method)
                => new(method.GetName(), method.GetParameters().Select(x => x.GetParameter2()), TType.Parse(method.ReturnType), System.Array.Empty<Dependency>());

        public static IEnumerable<PropertyDeclarationSyntax> GetProperties(this CSharpSyntaxNode node)
            => node.DescendantNodes().OfType<PropertyDeclarationSyntax>();

        public static IEnumerable<VariableDeclarationSyntax> GetVariables(this CSharpSyntaxNode node)
            => node.DescendantNodes().OfType<VariableDeclarationSyntax>();

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

        //public static IEnumerable<FieldDeclarationSyntax> GetType(this FieldDeclarationSyntax node)
        //{
        //    var x = node.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
        //    SemanticModel
        //}

        public static ISymbol GetTType(this CSharpSyntaxNode node, string? key = null) => GetContext(key).SemanticModel.GetSymbolInfo(node).Symbol!;


        //public static Method Params(this MethodDeclarationSyntax method)
        //    => method.ParameterList.
        //public static Method Types(this ParameterListSyntax parameterList)
        //    => parameterList.Parameters
        public static Parameter GetParameter2(this ParameterSyntax parameter)
           => new(parameter.Identifier.ValueText, TType.Parse(parameter.Type!));
        ////public static Parameter GetPType(this ParameterSyntax parameter)
        ////   => SemanticModel.GetDeclaredSymbol(parameter);

        ////public static Namespace GetModel(this AttributeListSyntax attributeList)
        ////    => new Parameter(attributeList., attributeList.Type.GetModel());

        private static CompilationContext GetContext(SyntaxTree tree, string? key = null)
        {
            if (key != null) return GetContext(key);
            var context = treeContexts.ContainsKey(tree) ? treeContexts[tree] : new CompilationContext();
            treeContexts[tree] = context;
            return context;
        }

        private static CompilationContext GetContext(string? key = null)
        {
            key = key ?? lastKey;
            var context = contexts.ContainsKey(key) ? contexts[key] : new CompilationContext();
            contexts[key] = context;
            lastKey = key;
            return context;
        }
    }
}