using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Models;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

public static class ModelFinder
{
    public static void Execute(GeneratorExecutionContext context)
    {
        //var models = classes.Where(x => x.AttributeLists.FirstOrDefault()?.GetType() == typeof(ModelAttribute));
        // add the filepath of each tree to the class we're building
        //foreach (var model in classes)
        //{
        //    sourceBuilder.AppendLine($@"Console.WriteLine(@""{CleanString(model.ToString())}"");");
        //}
    }

    public static IEnumerable<SyntaxTree> Trees(this GeneratorExecutionContext context) => context.Compilation.SyntaxTrees;
    public static SemanticModel Model(this ClassDeclarationSyntax @class, GeneratorExecutionContext context) => context.Compilation.GetSemanticModel(@class.SyntaxTree, true);
    public static ITypeSymbol Type(this ClassDeclarationSyntax @class, GeneratorExecutionContext context)
        => @class.Model(context).GetDeclaredSymbol(@class) as ITypeSymbol ?? throw new ArgumentException($"No type Symbol found for class '{@class}'");
    public static List<ClassDeclarationSyntax> Classes(this IEnumerable<SyntaxTree> trees, ClassFilter? filter = null)
        => new ClassVirtualizationVisitor(filter).GetClasses(trees);
    public static List<ClassDeclarationSyntax> Classes(this GeneratorExecutionContext context, ClassFilter? filter = null)
        => context.Trees().Classes(filter);
    public static List<ClassDeclarationSyntax> GetModelClasses(this GeneratorExecutionContext context)
        => context.Trees().GetModelClasses();
    public static List<ClassDeclarationSyntax> GetModelClasses(this IEnumerable<SyntaxTree> trees)
        => trees.Classes(new ClassFilter(attributes: new Type[] { typeof(ModelAttribute) }));
    //public static RecordDeclarationSyntax ToRecord(this ClassDeclarationSyntax @class)
    //    => trees.Classes(new ClassFilter(attributes: new Type[] { typeof(ModelAttribute) }));

    public static void Find(PropertyDeclarationSyntax property)
    {
        var name = property.Identifier.ToString();
        var type = property.Type;
    }

    public static string CleanString(string s) => s
        .Replace("\\", "\\\\")
        .Replace("\"", "\"\"");
}