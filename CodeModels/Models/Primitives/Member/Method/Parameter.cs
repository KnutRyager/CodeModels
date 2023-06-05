using System.Collections.Generic;
using CodeModels.Models.Primitives.Attribute;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Utils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member
{
    public record Parameter(string Name, IType Type, IExpression? Expression, AttributeList Attributes, Modifier Modifier)
        : CodeModel<ParameterSyntax>(), IToParameterListConvertible, INamedValue
    {
        public IExpression Value => Expression ?? NullValue;

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

        public ParameterSyntax TypelessSyntax()
            => SyntaxFactory.Parameter(SyntaxFactory.List(new List<AttributeListSyntax>()),
                Modifier.Syntax(),
                default,
                Identifier(Name),
                Expression is null || ExpressionUtils.IsVoid(Expression) ? null : EqualsValueClause(Expression.Syntax()));

        public Argument ToArgument() => Arg(ToExpression());
        public ArgumentList ToArgumentList() => ArgList(ToArgument());
        public IExpression ToExpression() => Expression ?? NullValue;
        public Parameter ToParameter() => this;
        public ParameterList ToParameterList() => ParamList(this);
        public Property ToProperty() => Property(Name, ToExpression());

        public TupleElementSyntax ToTupleElement()
        {
            throw new System.NotImplementedException();
        }

        public IType ToType()
        {
            throw new System.NotImplementedException();
        }
    }
}