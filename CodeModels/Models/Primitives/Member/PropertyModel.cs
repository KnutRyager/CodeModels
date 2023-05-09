using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.Models.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models;

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

    public override IInvocation AccessValue(IExpression? instance = null) => new PropertyModelExpression(this, instance);

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

    public InvocationExpression Invoke(IExpression? caller) => CodeModelFactory.Invocation(GetGetter() ?? throw new ArgumentException("No getter"), caller, null, GetScopes());
    public override IInvocation Invoke(IExpression? caller, IEnumerable<IExpression> _)
        => Invoke(caller);

    public Method? GetGetter()
        => Owner is ITypeDeclaration b ? b.Methods().FirstOrDefault(x => ((IMemberInfo)x).Name == $"get_{Name}") as Method
        : Accessors.FirstOrDefault(x => x.Type is AccessorType.Get)?.GetMethod(Name);

    public Method? GetSetter()
        => Owner is ITypeDeclaration b ? b.Methods().FirstOrDefault(x => ((IMember)x).Name == $"set_{Name}") as Method
        : Accessors.FirstOrDefault(x => x.Type is AccessorType.Set or AccessorType.Init)?.GetMethod(Name);

    public FieldModel? GetBackingField()
        => Owner is ClassDeclaration b ? b.GetFields().FirstOrDefault(x => ((IMember)x).Name == AccessorType.Get.GetBackingFieldName(Name)) : null;

    public override IExpression EvaluateAccess(IExpression expression, IProgramModelExecutionContext context)
    {
        var scopes = GetScopes(expression);
        try
        {
            context.EnterScopes(scopes);
            return context.GetValue(Name);
        }
        finally
        {
            context.ExitScopes(scopes);
        }
        //if (expression is InstantiatedObject instance)
        //{
        //    instance.EnterScopes(context);
        //}
        //else if (Owner is ClassDeclaration baseType)
        //{
        //    context.EnterScope(baseType.GetStaticScope());
        //}
        //try
        //{
        //    var getter = GetGetter();

        //    if (getter is not null)
        //    {
        //        return CodeModelFactory.Invocation(getter, expression).Evaluate(context);
        //    }
        //    else
        //    {
        //        var backingField = GetBackingField()!;
        //        return new FieldModelExpression(backingField, expression, GetScopes(expression)).Evaluate(context);
        //    }
        //}
        //finally
        //{
        //    if (expression is InstantiatedObject instanceExit)
        //    {
        //        instanceExit.ExitScopes(context);
        //    }
        //    else if (Owner is ClassDeclaration)
        //    {
        //        context.ExitScope();
        //    }
        //}
    }

    public override void Assign(IExpression instance, IExpression value, IProgramModelExecutionContext context)
    {
        var setter = GetSetter();
        if (setter is not null)
        {
            CodeModelFactory.Invocation(setter, instance, new IExpression[] { value }, GetScopes(instance)).Evaluate(context);
        }
        else
        {
            var backingField = GetBackingField()!;
            new FieldModelExpression(backingField, instance, GetScopes(instance)).Assign(value).Evaluate(context);
        }
    }

    public virtual void Assign(IExpression value, IProgramModelExecutionContext context, IList<IProgramModelExecutionScope> scopes)
    {
        try
        {
            context.EnterScopes(scopes);
            Assign(value).Evaluate(context);
        }
        finally
        {
            context.ExitScopes(scopes);
        }
    }

    public AssignmentExpression Assign(IExpression value) => ToIdentifierExpression().Assign(value);
    public AssignmentExpression Assign(IExpression? caller, IExpression value) => CodeModelFactory.Assignment(
        CodeModelFactory.MemberAccess(caller ?? Owner?.ToIdentifierExpression() ?? throw new NotImplementedException(),
            ToIdentifierExpression()), value);
}

