using System.Collections.Generic;
using System.Linq;
using Common.Util;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models
{
    public record Method(string Name, PropertyCollection Parameters, IType ReturnType, List<Dependency> Dependencies, Modifier Modifier = Modifier.Public)
        : IMember, ICodeModel
    {
        public Method(string name, PropertyCollection parameters, IType returnType, IEnumerable<Dependency>? dependencies = null)
            : this(name, parameters, returnType, dependencies?.ToList() ?? new List<Dependency>()) { }

        public Method(MethodDeclarationSyntax method) : this(method.GetName(), new PropertyCollection(method), AbstractType.Parse(method.ReturnType)) { }

        public MethodDeclarationSyntax ToMethod(Modifier modifiers = Modifier.None, Modifier removeModifier = Modifier.None) => MethodDeclarationCustom(
            attributeLists: default,
            modifiers: Modifier.SetModifiers(modifiers).SetFlags(removeModifier, false).Modifiers(),
            returnType: ReturnType.TypeSyntax(),
            explicitInterfaceSpecifier: default,
            identifier: Identifier(Name),
            typeParameterList: default,
            parameterList: Parameters.ToParameters(),
            constraintClauses: default,
            body: default,
            expressionBody: default);

        public MemberDeclarationSyntax ToMemberSyntax(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None) => ToMethod(modifier, removeModifier);
        public CSharpSyntaxNode SyntaxNode() => ToMemberSyntax();
    }
}
