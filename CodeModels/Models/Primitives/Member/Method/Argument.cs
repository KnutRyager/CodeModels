using System.Collections.Generic;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Reference;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member
{
    // TODO: refKindKeyword
    public record Argument(string? Name, IExpression Expression)
        : CodeModel<ArgumentSyntax>(), IToArgumentListConvertible, IExpression
    {
        public bool IsLiteralExpression => throw new System.NotImplementedException();

        public static Argument Create(string? name, IExpression value)
            => new(name, value);

        public override IEnumerable<ICodeModel> Children()
        {
            yield return Expression;
        }


        public ExpressionStatement AsStatement() => Expression.AsStatement();
        public IExpression Evaluate(ICodeModelExecutionContext context) => Expression.Evaluate(context);
        public object? EvaluatePlain(ICodeModelExecutionContext context) => Expression.EvaluatePlain(context);
        public IType Get_Type() => Expression.Get_Type();
        public IdentifierNameSyntax IdentifierNameSyntax() => Expression.IdentifierNameSyntax();
        public SyntaxToken IdentifierSyntax() => Expression.IdentifierSyntax();
        public LiteralExpressionSyntax? LiteralSyntax() => Expression.LiteralSyntax();
        public object? LiteralValue() => Expression.LiteralValue();
        public SimpleNameSyntax NameSyntax() => Expression.NameSyntax();

        public override ArgumentSyntax Syntax()
            => SyntaxFactory.Argument(Name is null ? null : NameColon(IdentifierName(Name)), Token(SyntaxKind.None), Expression.Syntax());

        public Argument ToArgument() => this;
        public ArgumentList ToArgumentList() => ArgList(new[] { this });

        public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => Expression.ToEnumValue();
        public IdentifierExpression ToIdentifierExpression() => Expression.ToIdentifierExpression();
        ExpressionSyntax IExpression.Syntax() => Expression.Syntax();
        ExpressionOrPatternSyntax IExpressionOrPattern.Syntax() => Expression.Syntax();
    }
}
