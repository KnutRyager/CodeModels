using System.Collections.Generic;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.AbstractCodeModels.Member;
using CodeModels.Execution.Context;
using CodeModels.Execution.Scope;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Member;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeModels.Models
{
    public interface IInterfaceDeclaration : ITypeDeclaration<InterfaceDeclarationSyntax>
    {
        IInterfaceDeclaration? Parent { get; }
        List<AbstractProperty> AsList(AbstractProperty? typeSpecifier = null);
        IType BaseType();
        List<IType> ConvertToList();
        CodeModelExecutionScope CreateInstanceScope(bool init = false);
        void Evaluate(ICodeModelExecutionContext context);
        List<Property> FilterValues();
        List<IMember> Ordered();
        SeparatedSyntaxList<ExpressionSyntax> SyntaxList();
        ArgumentListSyntax ToArguments();
        ArrayCreationExpressionSyntax ToArrayCreationSyntax();
        ClassDeclarationSyntax ToClass(string? name = null, Modifier modifiers = Modifier.Public, Modifier memberModifiers = Modifier.Public);
        List<IExpression> ToExpressions();
        InitializerExpressionSyntax ToInitializer();
        SyntaxList<MemberDeclarationSyntax> ToMembers(Modifier modifier = Modifier.None);
        NamedValueCollection ToNamedValues();
        ParameterListSyntax ToParameters();
        RecordDeclarationSyntax ToRecord(string? name = null, Modifier modifiers = Modifier.Public);
        ITypeCollection ToTypeCollection();
        ExpressionCollection ToValueCollection();
    }
}