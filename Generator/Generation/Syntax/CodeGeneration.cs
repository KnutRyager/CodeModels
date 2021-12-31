using System;
using System.CodeDom;
using System.Linq;
using System.Text.RegularExpressions;
using CodeAnalyzation.Models.ErDiagram;
using CodeAnalyzation.Rewriters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Generation
{
    public static class CodeGeneration
    {
        public static MemberDeclarationSyntax GeneratePOJOFile(Clazz clazz, string? @namespace = null) =>
            @namespace == null ? GeneratePOJO(clazz) : NamespaceDeclaration(ParseName(@namespace)).AddMembers(GeneratePOJO(clazz));

        public static ClassDeclarationSyntax GeneratePOJO(Clazz clazz) => ClassDeclaration(
            identifier: clazz.Name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers(clazz.Fields.Select(Property).ToArray())
            .NormalizeWhitespace().NormalizeWhitespacesSingleLineProperties();

        public static PropertyDeclarationSyntax Property(Field field) => PropertyDeclaration(
            field.Type != null ? TypeSyntax(field.Type) : ParseTypeName(field.ReferenceClazz?.Name ?? throw new ArgumentException($"NO reference class for {field}.")), field.Name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

        public static MemberDeclarationSyntax GenerateCRUDApi(Clazz clazz, string? @namespace = null) => ClassDeclaration(
            identifier: $"{clazz.Name}Api")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddMembers(clazz.Fields.Select(Property).ToArray())
            .NormalizeWhitespace().NormalizeWhitespacesSingleLineProperties();

        public static MethodDeclarationSyntax GenerateCRUDApiMethod(Clazz clazz)
        {
            return MethodDeclaration(ParseTypeName("void"), $"{clazz.Name}Api")

    .AddModifiers(Token(SyntaxKind.PublicKeyword))
    .NormalizeWhitespace().NormalizeWhitespacesSingleLineProperties();
        }

        //public static MethodDeclarationSyntax GenerateMethodImplementation(Clazz clazz)
        //{

        //}


        public static TypeSyntax TypeSyntax(Type type) => ParseTypeName(GetPrimitive(type));

        public static string GetPrimitive(Type t)
        {
            string typeName;
            using (var provider = new CSharpCodeProvider())
            {
                var typeRef = new CodeTypeReference(t);
                typeName = provider.GetTypeOutput(typeRef);
            }
            if (typeName.StartsWith("System.Nullable<"))
            {
                typeName = $"{typeName[16..^1]}?";
            }
            return typeName;
        }

        private static readonly Regex AutoPropRegex = new(@"\s*\{\s*get;\s*set;\s*}\s");

        public static string FormatAutoPropertiesOnOneLine(this string str)
            => AutoPropRegex.Replace(str, " { get; set; }");
    }
}