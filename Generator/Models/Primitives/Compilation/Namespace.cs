using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataStructures;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record Namespace(string Identifier, INamespaceSymbol? Symbol = null) : CodeModel<NamespaceDeclarationSyntax>(), IType
{
    public string Name => Identifier;
    public bool Required => throw new NotImplementedException();
    public bool IsMulti => throw new NotImplementedException();
    public bool IsStatic => throw new NotImplementedException();
    public Type? ReflectedType => throw new NotImplementedException();
    public EqualityList<IType> GenericTypes => throw new NotImplementedException();
    public IType Get_Type() => this;
    public bool IsLiteralExpression => throw new NotImplementedException();
    public LiteralExpressionSyntax? LiteralSyntax => null;
    public object? LiteralValue => throw new NotImplementedException();

    public Namespace(IEnumerable<string> parts) : this(string.Join(".", parts)) { }
    public Namespace(INamespaceSymbol Symbol) : this(Symbol.ToString(), Symbol) { }
    public Namespace(params string[] parts) : this(parts.ToList()) { }

    public Namespace(NamespaceDeclarationSyntax @namespace) : this(string.IsNullOrWhiteSpace(@namespace.Name.ToString()) ? new string[] { }
    : new[] { @namespace.Name.ToString() })
    { }

    public override NamespaceDeclarationSyntax Syntax() => NamespaceDeclaration(IdentifierName(Identifier));

    public override IEnumerable<ICodeModel> Children()
    {
        throw new NotImplementedException();
    }

    TypeSyntax IType.Syntax() => new QuickType(Name).Syntax();
    public TypeSyntax TypeSyntaxNonMultiWrapped() => new QuickType(Name).Syntax();
    public TypeSyntax TypeSyntaxNullableWrapped(TypeSyntax type) => new QuickType(Name).Syntax();
    public TypeSyntax TypeSyntaxMultiWrapped(TypeSyntax type) => new QuickType(Name).Syntax();
    public TypeSyntax TypeSyntaxUnwrapped() => new QuickType(Name).Syntax();
    public TypeParameterSyntax ToTypeParameter() => throw new NotImplementedException();
    public Type? GetReflectedType() => throw new NotImplementedException();
    public IType ToMultiType() => throw new NotImplementedException();
    public string GetMostSpecificType() => throw new NotImplementedException();
    public IType GetGenericType(int index) => throw new NotImplementedException();
    TypeSyntax ICodeModel<TypeSyntax>.Syntax() => throw new NotImplementedException();
    public ArgumentSyntax ToArgument() => throw new NotImplementedException();
    public IExpression Evaluate(IProgramModelExecutionContext context) => this;
    public object? EvaluatePlain(IProgramModelExecutionContext context) => throw new NotImplementedException();
    public EnumMemberDeclarationSyntax ToEnumValue(int? value = null) => throw new NotImplementedException();
    public ExpressionStatement AsStatement() => throw new NotImplementedException();
    ExpressionSyntax IExpression.Syntax() => throw new NotImplementedException();

    public IdentifierExpression GetIdentifier() => new(Identifier, Symbol: Symbol);
}
