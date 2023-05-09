using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.Execution.Context;
using CodeModels.Factory;
using CodeModels.Models.Primitives.Expression.Abstract;
using Common.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

public record Namespace(
    string Name, 
    INamespaceSymbol? Symbol = null)
    : NamedCodeModel<NamespaceDeclarationSyntax>(Name), IType
{
    public bool Required => throw new NotImplementedException();
    public bool IsMulti => throw new NotImplementedException();
    public bool IsStatic => throw new NotImplementedException();
    public Type? ReflectedType => throw new NotImplementedException();
    public EqualityList<IType> GenericTypes => throw new NotImplementedException();
    public IType Get_Type() => this;
    public bool IsLiteralExpression => throw new NotImplementedException();
    public LiteralExpressionSyntax? LiteralSyntax() => null;
    public object? LiteralValue() => throw new NotImplementedException();

    public Modifier Modifier => throw new NotImplementedException();

    public string TypeName => throw new NotImplementedException();

    public Namespace(IEnumerable<string> parts) : this(string.Join(".", parts)) { }
    public Namespace(INamespaceSymbol Symbol) : this(Symbol.ToString(), Symbol) { }
    public Namespace(params string[] parts) : this(parts.ToList()) { }

    public Namespace(NamespaceDeclarationSyntax @namespace) : this(string.IsNullOrWhiteSpace(@namespace.Name.ToString()) ? new string[] { }
    : new[] { @namespace.Name.ToString() })
    { }

    public override NamespaceDeclarationSyntax Syntax() => NamespaceDeclaration(IdentifierNameSyntax());

    public override IEnumerable<ICodeModel> Children()
    {
        throw new NotImplementedException();
    }

    TypeSyntax IType.Syntax() => CodeModelFactory.QuickType(Name).Syntax();
    public TypeSyntax TypeSyntaxNonMultiWrapped() => CodeModelFactory.QuickType(Name).Syntax();
    public TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type) => CodeModelFactory.QuickType(Name).Syntax();
    public TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type) => CodeModelFactory.QuickType(Name).Syntax();
    public TypeSyntax TypeSyntaxUnwrapped() => CodeModelFactory.QuickType(Name).Syntax();
    public TypeParameterSyntax ToTypeParameter() => throw new NotImplementedException();
    public Type? GetReflectedType() => throw new NotImplementedException();
    public IType ToMultiType() => throw new NotImplementedException();
    public string GetMostSpecificType() => throw new NotImplementedException();
    public IType GetGenericType(int index) => throw new NotImplementedException();
    TypeSyntax ICodeModel<TypeSyntax>.Syntax() => throw new NotImplementedException();
    public ArgumentSyntax ToArgument() => throw new NotImplementedException();
    public IExpression Evaluate(ICodeModelExecutionContext context) => this;
    public object? EvaluatePlain(ICodeModelExecutionContext context) => throw new NotImplementedException();
    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => throw new NotImplementedException();
    public ExpressionStatement AsStatement() => throw new NotImplementedException();
    ExpressionSyntax IExpression.Syntax() => throw new NotImplementedException();
    ExpressionOrPatternSyntax IExpressionOrPattern.Syntax() => throw new NotImplementedException();

    MemberDeclarationSyntax IMember.Syntax()
    {
        throw new NotImplementedException();
    }

    public MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
    {
        throw new NotImplementedException();
    }

    public TypeSyntax TypeSyntax()
    {
        throw new NotImplementedException();
    }
    public bool Equals(IType other, ICodeModelExecutionContext context)
        => TypeName == other.TypeName; // TODO: Check assembly
    public bool IsAssignableFrom(IType other, ICodeModelExecutionContext context)
        => Equals(other);

    public ICodeModel Render(Namespace @namespace)
    {
        throw new NotImplementedException();
    }

    public IType ToType()
    {
        throw new NotImplementedException();
    }

    public IExpression ToExpression()
    {
        throw new NotImplementedException();
    }

    public ParameterSyntax ToParameter()
    {
        throw new NotImplementedException();
    }

    public TupleElementSyntax ToTupleElement()
    {
        throw new NotImplementedException();
    }

    public IType PlainType()
    {
        throw new NotImplementedException();
    }
}
