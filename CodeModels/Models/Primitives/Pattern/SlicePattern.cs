//using System.Collections.Generic;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//namespace CodeModels.Models;

//public abstract record SlicePattern(IExpression Expression)
//    : Pattern<SlicePatternSyntax>
//{
//    public override IEnumerable<ICodeModel> Children()
//    {
//        yield return Expression;
//    }

//    public override SlicePatternSyntax Syntax()
//        => SyntaxFactory.SlicePattern(Expression.Syntax());
//}