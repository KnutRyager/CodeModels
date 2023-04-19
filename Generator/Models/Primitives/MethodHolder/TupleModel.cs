using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Models.ErDiagram;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeAnalyzation.Models.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

//public record TupleModel(List<FieldModel> Fields, IType? SpecifiedType = null, Modifier Modifier = Modifier.Public)
//    : BaseType<TupleTypeSyntax>(
//        Name: "Tuple",
//        Members: Fields.Select(x => (IFieldOrProperty)x).ToList(),
//        Methods: List<IMethod>(),
//        Namespace: null,
//        TopLevelModifier: Modifier.None,
//        MemberModifier: Modifier,
//        ReflectedType: null),
//    IMember
//{
//    public override IEnumerable<ICodeModel> Children()
//    {
//        foreach (var property in Fields) yield return property;
//    }

//    //public override IExpression Evaluate(IProgramModelExecutionContext context) => Literal(ToExpressions().Select(x => x.EvaluatePlain(context)).ToArray());

//    public IType BaseType() => new QuickType(Name);

//    public override TupleTypeSyntax Syntax() => ToTupleType();
//}

