using CodeAnalyzation.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeAnalyzation
{
    public static class SyntaxNodeExtensions
    {
        private static CSharpCompilation? _compilation;
        public static CSharpCompilation Compilation => _compilation ?? throw new Exception("_compilation not initialized.");

        private static SemanticModel? _semanticModel;
        private static SemanticModel SemanticModel => _semanticModel ?? throw new Exception("_semanticModel not initialized.");

        public static void SetCompilation(CSharpCompilation compilation, IEnumerable<SyntaxTree> trees)
        {
            _compilation = compilation;
            _semanticModel = GetSemanticModel(trees.FirstOrDefault());
        }

        public static void SetSemanticModel(SyntaxTree tree)
            => _semanticModel = GetSemanticModel(tree);

        public static void SetSemanticModel(int treeIndex)
            => _semanticModel = GetSemanticModel(Compilation.SyntaxTrees[treeIndex]);

        public static SemanticModel GetSemanticModel(SyntaxTree? tree = null) => (tree == null)! ? SemanticModel : Compilation.GetSemanticModel(tree);

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
                   => new(method.GetName(), method.GetParameters().Select(x => x.GetParameter2()), method.ReturnType.GetType2(), System.Array.Empty<Dependency>());


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

        public static ISymbol GetTType(this CSharpSyntaxNode node) => SemanticModel.GetSymbolInfo(node).Symbol!;


        //public static Method Params(this MethodDeclarationSyntax method)
        //    => method.ParameterList.
        //public static Method Types(this ParameterListSyntax parameterList)
        //    => parameterList.Parameters
        public static Parameter GetParameter2(this ParameterSyntax parameter)
           => new(parameter.Identifier.ValueText, parameter.Type!.GetType2());
        ////public static Parameter GetPType(this ParameterSyntax parameter)
        ////   => SemanticModel.GetDeclaredSymbol(parameter);

        ////public static Namespace GetModel(this AttributeListSyntax attributeList)
        ////    => new Parameter(attributeList., attributeList.Type.GetModel());

        public static TType GetType2(this TypeSyntax type) => new(type.ToString());
    }
}