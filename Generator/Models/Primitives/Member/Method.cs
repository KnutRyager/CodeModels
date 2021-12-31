using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;

namespace CodeAnalyzation.Models
{
    public record Method(string Name, PropertyCollection Parameters, IType ReturnType, List<Dependency> Dependencies) : IMember
    {
        public Method(string name, PropertyCollection parameters, IType returnType, IEnumerable<Dependency>? dependencies = null)
            : this(name, parameters, returnType, dependencies?.ToList() ?? new List<Dependency>()) { }

        public Method(MethodDeclarationSyntax method) : this(method.GetName(), new PropertyCollection(method), AbstractType.Parse(method.ReturnType)) { }

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
