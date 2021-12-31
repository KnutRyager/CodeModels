using System;
using System.Collections.Generic;
using CodeAnalyzation.Collectors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Models;

namespace CodeAnalyzation.Generation
{
    public static class GeneratorExtensions
    {
        public static IEnumerable<SyntaxTree> Trees(this GeneratorExecutionContext context) => context.Compilation.SyntaxTrees;
        public static SemanticModel Model(this ClassDeclarationSyntax @class, GeneratorExecutionContext context) => context.Compilation.GetSemanticModel(@class.SyntaxTree, true);
        public static ITypeSymbol Type(this ClassDeclarationSyntax @class, GeneratorExecutionContext context)
            => @class.Model(context).GetDeclaredSymbol(@class) as ITypeSymbol ?? throw new ArgumentException($"No type Symbol found for class '{@class}'");
        public static List<ClassDeclarationSyntax> Classes(this IEnumerable<SyntaxTree> trees, ClassFilter? filter = null)
            => new ClassVisiter(filter).GetEntries(trees);
        public static List<ClassDeclarationSyntax> Classes(this GeneratorExecutionContext context, ClassFilter? filter = null)
            => context.Trees().Classes(filter);
        public static List<ClassDeclarationSyntax> GetModelClasses(this GeneratorExecutionContext context)
            => context.Trees().GetModelClasses();
        public static List<ClassDeclarationSyntax> GetModelClasses(this IEnumerable<SyntaxTree> trees)
            => trees.Classes(new ClassFilter(attributes: new Type[] { typeof(ModelAttribute) }));
    }
}