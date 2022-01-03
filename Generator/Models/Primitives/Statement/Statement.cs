using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Models.CodeModelFactory;
using static CodeAnalyzation.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis;

namespace CodeAnalyzation.Models
{

    //public record VariableDeclaration(IType Type, string Name, Expression Value) : Statement<VariableDeclarationSyntax>
    //{
    //    public override VariableDeclarationSyntax Statement<VariableDeclarationSyntax>.SyntaxNode() => VariableDeclarationCustom(Type.Syntax!, VariableDeclaratorCustom(Identifier(Name)));
    //}

    //public record LocalDeclarationStatement(IType Type, string Name, IExpression Value) : AbstractStatement<LocalDeclarationStatementSyntax>
    //{
    //    public override LocalDeclarationStatementSyntax SyntaxNode() => LocalDeclarationStatementCustom(Type.Syntax!, VariableDeclaratorCustom(Identifier(Name)));
    //}

    public record Block(List<IStatement> Statements) : AbstractStatement<BlockSyntax>
    {
        public Block(IEnumerable<IStatement>? statements) : this(List(statements)) { }
        public override BlockSyntax Syntax() => Block(Statements.Select(x => x.Syntax()));
    }

    //public record ForStatement(Block Block, IExpression Condition, IExpression Initializer) : AbstractStatement<ForStatementSyntax>
    //{
    //    public ForStatement(ForStatementSyntax Syntax) { }
    //    public override ForStatementSyntax SyntaxNode() => ForStatementCustom(Declaration,
    //        List(Initializer), 
    //        Condition.SyntaxNode(),
    //        incrementors, 
    //        Block.SyntaxNode());

    //}
    //public static ForStatementSyntax ForStatementCustom(
    //        VariableDeclarationSyntax? declaration,
    //        IEnumerable<ExpressionSyntax> initializers,
    //        ExpressionSyntax? condition,
    //        IEnumerable<ExpressionSyntax> incrementors,
    //        StatementSyntax statement)

}