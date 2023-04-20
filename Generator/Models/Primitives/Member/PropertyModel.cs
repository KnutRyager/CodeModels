using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Models.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeAnalyzation.Models;

public record PropertyModel(string Name,
    IType Type,
    List<AttributeList> Attributes,
    List<Accessor> Accessors,
    Modifier Modifier,
    IExpression Value)
    : FieldOrProperty<PropertyDeclarationSyntax>(Name, Type, Attributes, Modifier, Value)
{
    public static PropertyModel Create(string name,
    IType type,
    IEnumerable<Accessor> accessors,
    IEnumerable<AttributeList>? attributes = null,
    Modifier modifier = Modifier.Public,
    IExpression? value = null) => new(name,
    type,
    CodeModelFactory.List(attributes),
    CodeModelFactory.List(accessors),
    modifier,
    value ?? CodeModelFactory.VoidValue);

    public override IEnumerable<ICodeModel> Children()
    {
        yield return Type;
        foreach (var accessor in Accessors) yield return accessor;
        foreach (var attribute in Attributes) yield return attribute;
    }

    public Accessor? GetAccessor => Accessors.FirstOrDefault(x => x.Type is AccessorType.Get);
    public Accessor? SetAccessor => Accessors.FirstOrDefault(x => x.Type is AccessorType.Set);
    public Accessor? InitAccessor => Accessors.FirstOrDefault(x => x.Type is AccessorType.Init);

    public override IExpression AccessValue(IExpression? instance = null) => new PropertyModelExpression(this, instance);

    public override PropertyDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => PropertyDeclaration(
            attributeLists: List(Attributes.Select(x => x.Syntax())),
            modifiers: Modifier.Syntax(),
            type: Type.Syntax(),
            explicitInterfaceSpecifier: default,    // TODO
            identifier: ToIdentifier(),
            accessorList: AccessorList(IsGetOnly() ? List<AccessorDeclarationSyntax>() : List(Accessors.Select(x => x.Syntax()))),
            expressionBody: IsGetOnly()
                ? GetterExpressionBody() is IExpression getter
                    ? ArrowExpressionClause(getter.Syntax()) : null : null,
            initializer: Initializer());

    public bool IsGetOnly() => Accessors.Count is 1 && GetAccessor is not null;

    public IExpression? GetterExpressionBody() => GetAccessor is Accessor getter
        ? getter.ExpressionBody : null;

    public InvocationExpression Invoke(IExpression caller) => CodeModelFactory.Invocation(GetGetter() ?? throw new ArgumentException("No getter"), caller, null);

    public Method? GetGetter()
        => Owner is IMethodHolder b ? b.Methods().FirstOrDefault(x => ((IMemberInfo)x).Name == $"get_{Name}") as Method
        : Accessors.FirstOrDefault(x => x.Type is AccessorType.Get)?.GetMethod(Name);
    
    public Method? GetSetter()
        => Owner is IMethodHolder b ? b.Methods().FirstOrDefault(x => ((IMemberInfo)x).Name == $"set_{Name}") as Method
        : Accessors.FirstOrDefault(x => x.Type is AccessorType.Set or AccessorType.Init)?.GetMethod(Name);


    public virtual IExpression EvaluateAccess(IProgramModelExecutionContext context, IExpression instance)
    {
        throw new NotImplementedException();
    }

    public virtual void Assign(IExpression value, IProgramModelExecutionContext context)
    {
        throw new NotImplementedException();
    }
}

