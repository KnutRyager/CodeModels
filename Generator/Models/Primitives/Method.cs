using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models
{
    public class Method : IMember
    {
        public string Name { get; set; }
        public PropertyCollection Parameters { get; set; }
        public TType ReturnType { get; set; }
        public IEnumerable<Dependency> Dependencies { get; set; }

        public Method(string name, PropertyCollection parameters, TType returnType, IEnumerable<Dependency> dependencies)
        {
            Name = name;
            Parameters = parameters;
            ReturnType = returnType;
            Dependencies = dependencies;
        }

        public Method(MethodDeclarationSyntax method)
            : this(method.GetName(), new PropertyCollection(method), TType.Parse(method.ReturnType), System.Array.Empty<Dependency>()) { }

        public MethodDeclarationSyntax ToMethod() => MethodDeclarationCustom(
            attributeLists: default,
            modifiers: TokenList(Token(SyntaxKind.PublicKeyword)),
            returnType: ReturnType.TypeSyntax(),
            explicitInterfaceSpecifier: default,
            identifier: Identifier(Name),
            typeParameterList: default,
            parameterList: Parameters.ToParameters(),
            constraintClauses: default,
            body: default,
            expressionBody: default);
    }
}
