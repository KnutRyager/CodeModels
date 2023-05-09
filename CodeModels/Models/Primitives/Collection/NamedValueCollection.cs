using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeModels.Collectors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static CodeModels.Factory.CodeModelFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using CodeModels.Factory;
using CodeModels.Execution.Context;
using CodeModels.Models.Primitives.Expression.Abstract;

namespace CodeModels.Models;

public record NamedValueCollection(List<AbstractProperty> Properties, string? Name = null, IType? SpecifiedType = null)
    : Expression<ArrayCreationExpressionSyntax>(SpecifiedType ?? Type(TypeUtil.FindCommonType(Properties.Select(x => x.Value)), isMulti: true), Name: Name),
    INamedValueCollection<AbstractProperty>,
    IMember
{
    public NamedValueCollection(AbstractProperty property, string? name = null, IType? specifiedType = null) : this(List(property), name, specifiedType) { }
    public NamedValueCollection(IEnumerable<AbstractProperty>? properties = null, string? name = null, IType? specifiedType = null) : this(List(properties), name, specifiedType) { }
    public NamedValueCollection(IEnumerable<PropertyInfo> properties) : this(properties.Select(x => new PropertyFromReflection(x))) { }
    public NamedValueCollection(IEnumerable<FieldInfo> fields) : this(fields.Select(x => new PropertyFromField(x))) { }
    public NamedValueCollection(Type type) : this(type.GetProperties(), type.GetFields()) { }
    public NamedValueCollection(IEnumerable<PropertyInfo> properties, IEnumerable<FieldInfo> fields)
        : this(properties.Select(x => new PropertyFromReflection(x)).ToList<AbstractProperty>().Concat(fields.Select(x => new PropertyFromField(x)))) { }
    public NamedValueCollection(ClassDeclarationSyntax declaration) : this(new PropertyVisiter().GetEntries(declaration.SyntaxTree).Select(x => new AbstractProperty(x)), declaration.Identifier.ToString()) { }
    public NamedValueCollection(RecordDeclarationSyntax declaration) : this(new ParameterVisiter().GetEntries(declaration.SyntaxTree).Select(x => new AbstractProperty(x)), declaration.Identifier.ToString()) { }
    public NamedValueCollection(TupleTypeSyntax declaration) : this(new TupleElementVisiter().GetEntries(declaration.SyntaxTree).Select(x => new AbstractProperty(x))) { }
    public NamedValueCollection(MethodDeclarationSyntax declaration) : this(declaration.ParameterList) { }
    public NamedValueCollection(ConstructorDeclarationSyntax declaration) : this(declaration.ParameterList) { }
    public NamedValueCollection(ParameterListSyntax parameters) : this(parameters.Parameters.Select(x => new AbstractProperty(x))) { }
    public NamedValueCollection(IEnumerable<ParameterInfo> parameters) : this(parameters.Select(x => new PropertyFromParameter(x))) { }

    public ClassDeclarationSyntax ToClass(string? name = null, Modifier modifiers = Modifier.Public, Modifier memberModifiers = Modifier.Public) => ClassDeclarationCustom(
            attributeLists: default,
            modifiers: modifiers.Syntax(),
            identifier: name is null ? IdentifierSyntax() : CodeModelFactory.Identifier(name).ToToken(),
            typeParameterList: default,
            baseList: default,
            constraintClauses: default,
            members: ToMembers(memberModifiers)
        );

    public RecordDeclarationSyntax ToRecord(string? name = null, Modifier modifiers = Modifier.Public) => RecordDeclarationCustom(
            attributeLists: default,
            modifiers: modifiers.Syntax(),
            identifier: name is null ? IdentifierSyntax() : CodeModelFactory.Identifier(name).ToToken(),
            typeParameterList: default,
            parameterList: ToParameters(),
            baseList: default,
            constraintClauses: default,
            members: default);

    public TupleTypeSyntax ToTupleType() => TupleType(SeparatedList(Properties.Select(x => x.ToTupleElement())));
    public ParameterListSyntax ToParameters() => ParameterListCustom(Properties.Select(x => x.ToParameter()));
    public ArgumentListSyntax ToArguments() => ArgumentListCustom(Properties.Select(x => x.Value.ToArgument()));
    public InitializerExpressionSyntax ToInitializer() => InitializerExpression(SyntaxKind.ObjectCreationExpression, SeparatedList(Properties.Select(x => x.Value.Syntax())));
    public List<AbstractProperty> Ordered(Modifier modifier = Modifier.None) => Properties.OrderBy(x => x, new AbstractPropertyComparer(modifier)).ToList();
    public SyntaxList<MemberDeclarationSyntax> ToMembers(Modifier modifier = Modifier.None) => SyntaxFactory.List(Ordered(modifier).Select(x => x.SyntaxWithModifiers(modifier)));
    public List<AbstractProperty> FilterValues() => Properties.Where(x => x.Value != null).ToList();
    public List<IExpression> ToExpressions() => Properties.Select(x => x.Value).ToList();
    public ExpressionCollection ToValueCollection() => new(FilterValues().Select(x => x.Value ?? throw new Exception($"Property '{x}' contains no value.")), Type);
    public override ArrayCreationExpressionSyntax Syntax() => ToValueCollection().Syntax();
    public override LiteralExpressionSyntax? LiteralSyntax() => ToValueCollection().LiteralSyntax();
    public SeparatedSyntaxList<ExpressionSyntax> SyntaxList() => SeparatedList(Properties.Select(x => x.ExpressionSyntax!));
    public override object? LiteralValue() => ToValueCollection().LiteralValue();

    public Modifier Modifier => Modifier.Public;

    public bool IsStatic => false;

    public IExpression Value => VoidValue;

    public AbstractProperty this[string name] => Properties.First(x => x.Name == name);
    public INamed? TryFindNamedValue(string name) => Properties.FirstOrDefault(x => x.Name == name);

    public override IEnumerable<ICodeModel> Children()
    {
        foreach (var property in Properties) yield return property;
    }

    public override IExpression Evaluate(ICodeModelExecutionContext context) => Literal(ToExpressions().Select(x => x.EvaluatePlain(context)).ToArray());

    public IType BaseType() => CodeModelFactory.QuickType(Name);

    public List<IType> ConvertToList()
    => AsList().Select(x => x.ToType()).ToList();

    public List<AbstractProperty> AsList(AbstractProperty? typeSpecifier = null) => Properties;

    public ITypeCollection ToTypeCollection() => new TypeCollection(ConvertToList());

    MemberDeclarationSyntax IMember.Syntax()
        => ToClass();

    public MemberDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
        => ToMembers(modifier).First();

    public ICodeModel Render(Namespace @namespace)
        => this with { Name = @namespace.Name };

    public List<Property> ToFieldOrProperties() => Properties.Select(x => x.ToProperty()).ToList();

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

    public ClassDeclaration ToClassModel() => Class(Name ?? string.Empty, Properties.Select(x => x.ToProperty()));

    public Property ToProperty() => CodeModelFactory.Property(Name, Value);

    public static implicit operator NamedValueCollection(AbstractProperty property) => new(property);
}

