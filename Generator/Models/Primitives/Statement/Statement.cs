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
    public abstract record Statement(StatementSyntax Syntax) : IStatement<StatementSyntax>
    {
        public abstract StatementSyntax To();
    }

    public record VariableDeclaration(IType Type, string Name, Expression Value) : IStatement<VariableDeclarationSyntax>
    {
        public VariableDeclarationSyntax To() => VariableDeclarationCustom(Type.Syntax!, VariableDeclaratorCustom(Identifier(Name)));
        public CSharpSyntaxNode SyntaxNode() => To();
    }

    public record LocalDeclarationStatement(IType Type, string Name, Expression Value) : IStatement<LocalDeclarationStatementSyntax>
    {
        public LocalDeclarationStatementSyntax To() => LocalDeclarationStatementCustom(Type.Syntax!, VariableDeclaratorCustom(Identifier(Name)));
        public LocalDeclarationStatementSyntax SyntaxNode() => To();
    }

    public record Block(List<IStatement> Statements) 
    {
        public Block(IEnumerable<IStatement>? statements) : this(List(statements)) { }
        public BlockSyntax To() => Block(Statements.Select(x => x.To()));
        public CSharpSyntaxNode SyntaxNode() => To();
    }

    public record ForStatement(Expression Expression) : IStatement<ForStatementSyntax>
    {
        public ForStatement(ForStatementSyntax Syntax);
        public ForStatementSyntax To() => ForStatementCustom();
        public CSharpSyntaxNode SyntaxNode() => Statement();

    }


}