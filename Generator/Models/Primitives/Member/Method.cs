using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public record Method(string Name, PropertyCollection Parameters, IType ReturnType, List<Dependency> Dependencies, Modifier Modifier = Modifier.Public) : IMember, ICodeModel
    {
        public Method(string name, PropertyCollection parameters, IType returnType, IEnumerable<Dependency>? dependencies = null)
            : this(name, parameters, returnType, dependencies?.ToList() ?? new List<Dependency>()) { }

        public Method(MethodDeclarationSyntax method) : this(method.GetName(), new PropertyCollection(method), AbstractType.Parse(method.ReturnType)) { }

        public MethodDeclarationSyntax ToMethod() => MethodDeclarationCustom(
            attributeLists: default,
            modifiers: Modifier.Modifiers(),
            returnType: ReturnType.TypeSyntax(),
            explicitInterfaceSpecifier: default,
            identifier: Identifier(Name),
            typeParameterList: default,
            parameterList: Parameters.ToParameters(),
            constraintClauses: default,
            body: default,
            expressionBody: default);

        public MemberDeclarationSyntax ToMemberSyntax(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None) => ToMethod();
        public CSharpSyntaxNode SyntaxNode() => ToMemberSyntax();
    }
}
