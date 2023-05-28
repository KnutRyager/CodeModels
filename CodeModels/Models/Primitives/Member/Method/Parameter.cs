using System.Collections.Generic;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Utils;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member
{
    public record Parameter(string Name, IType Type, IExpression? Expression, AttributeList Attributes, Modifier Modifier)
        : CodeModel<ParameterSyntax>(), IToParameterListConvertible
    {
        public static Parameter Create(string name, IType type, IExpression? value = null, IToAttributeListConvertible? attributes = default)
            => new(name, type, value, attributes?.ToAttributeList() ?? Attributes(), Modifier.None);

        public override IEnumerable<ICodeModel> Children()
        {
            if (Expression is not null) yield return Expression;
        }

        public override ParameterSyntax Syntax()
            => SyntaxFactory.Parameter(SyntaxFactory.List(new List<AttributeListSyntax>()),
                // TODO Attributes
                //=> SyntaxFactory.Parameter(SyntaxFactory.List(new[] { Attributes.Syntax() }),
                Modifier.Syntax(),
                Type.Syntax(),
                Identifier(Name),
                Expression is null || ExpressionUtils.IsVoid(Expression) ? null : EqualsValueClause(Expression.Syntax()));

        public ParameterList ToParameterList() => ParamList(new[] { this });
    }
}