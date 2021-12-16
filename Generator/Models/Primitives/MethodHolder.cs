using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Models;
using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static CodeAnalyzation.CodeGeneration.SyntaxFactoryCustom;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace CodeAnalyzation.Models
{
    public class MethodHolder
    {
        public PropertyCollection Properties { get; set; }
        public List<Property> ValueProperties => Properties.FilterValues();
        public List<Method> Methods { get; set; }
        public string Name { get; set; }

        public MethodHolder(PropertyCollection properties, string name, IEnumerable<Method>? methods = null)
        {
            Properties = properties;
            Name = name;
            Methods = methods?.ToList() ?? new List<Method>();
        }

        public MethodHolder AddProperty(TType type, string name)
        {
            Properties.Properties.Add(new(type, name));
            return this;
        }

        public MethodHolder AddProperty(Type type, string name) => AddProperty(new TType(type), name);
        public MethodHolder AddProperty(ITypeSymbol type, string name) => AddProperty(new TType(type), name);

        public List<Property> GetReadonlyProperties() => Properties.Properties.Where(x => x.PropertyType.IsWritable()).ToList();
        public SyntaxList<MemberDeclarationSyntax> GetMethods() => List<MemberDeclarationSyntax>(Methods.Select(x => x.ToMethod()));
        public SyntaxList<MemberDeclarationSyntax> Members() => List(Properties.ToMembers().Concat(GetMethods()));

        public RecordDeclarationSyntax ToRecord() => RecordDeclarationCustom(
                attributeLists: default,
                modifiers: MethodHolderType.PublicRecord.Modifiers(),
                identifier: Identifier(Name),
                typeParameterList: default,
                parameterList: Properties.ToParameters(),
                baseList: default,
                constraintClauses: default,
                members: GetMethods());

        public ClassDeclarationSyntax ToClass() => ClassDeclarationCustom(
                attributeLists: default,
                modifiers: MethodHolderType.PublicClass.Modifiers(),
                identifier: Identifier(Name),
                typeParameterList: default,
                baseList: default,
                constraintClauses: default,
                members: Members());

        public StructDeclarationSyntax ToStruct() => StructDeclarationCustom(
                attributeLists: default,
                modifiers: MethodHolderType.PublicStruct.Modifiers(),
                identifier: Identifier(Name),
                typeParameterList: default,
                baseList: default,
                constraintClauses: default,
                members: Members());
    }
}