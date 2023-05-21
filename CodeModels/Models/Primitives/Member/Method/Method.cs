using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Factory;
using CodeModels.Models.Interfaces;
using CodeModels.Models.Primitives.Expression.Abstract;
using CodeModels.Models.Primitives.Expression.Invocation;
using CodeModels.Models.Reflection;
using Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.CodeModelFactory;
using static CodeModels.Generation.SyntaxFactoryCustom;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeModels.Models.Primitives.Member
{
    public record Method(string Name,
    NamedValueCollection Parameters,
    List<IType> GenericParameters,
    List<TypeParameterConstraintClause> ConstraintClauses,
    IType ReturnType,
    Block? Statements,
    IExpression? ExpressionBody = null,
        Modifier Modifier = Modifier.Public, List<AttributeList>? AttributesIn = null)
        : MethodBase<MethodDeclarationSyntax, InvocationExpression>(ReturnType, Name, AttributesIn ?? new List<AttributeList>(), Modifier),
        IMethod, IInvokable<InvocationExpression>
    {
        public static Method Create(string name,
            NamedValueCollection parameters,
            IType returnType,
            IEnumerable<IType>? genericParameters = null,
            IEnumerable<TypeParameterConstraintClause>? constraintClauses = null,
            Block? body = null,
            IExpression? expressionBody = null,
            Modifier? modifier = null)
            => new(name, parameters, List(genericParameters), List(constraintClauses), returnType, body, expressionBody, modifier ?? Modifier.Public);

        public MethodDeclarationSyntax ToMethodSyntax(Modifier modifiers = Modifier.None, Modifier removeModifier = Modifier.None) => MethodDeclarationCustom(
            attributeLists: new List<AttributeListSyntax>(),
            modifiers: Modifier.SetModifiers(modifiers).SetFlags(removeModifier, false).Syntax(),
            returnType: ReturnType.Syntax() ?? TypeShorthands.VoidType.Syntax()!,
            explicitInterfaceSpecifier: default,
            identifier: IdentifierSyntax(),
            typeParameterList: GenericParameters.Count is 0 ? null : TypeParameterList(SeparatedList(GenericParameters.Select(x => x.ToTypeParameter()))),
            parameterList: Parameters.ToParameters(),
            constraintClauses: SyntaxFactory.List(ConstraintClauses.Select(x => x.Syntax())),
            body: Statements?.Syntax(),
            expressionBody: ExpressionBody is null ? null : ArrowExpressionClause(ExpressionBody.Syntax()));

        public override InvocationExpression Invoke(IExpression? caller, IEnumerable<IExpression> arguments) => Invocation(this, caller, arguments);
        public InvocationExpression Invoke(IExpression caller, params IExpression[] arguments) => Invocation(this, caller, arguments);
        public InvocationExpression Invoke(string identifier, IEnumerable<IExpression> arguments) => Invoke(CodeModelFactory.Identifier(identifier), arguments);
        public InvocationExpression Invoke(string identifier, params IExpression[] arguments) => Invoke(CodeModelFactory.Identifier(identifier), arguments);
        public InvocationExpression Invoke(string identifier, IType? type, ISymbol? symbol, IEnumerable<IExpression> arguments) => Invoke(Identifier(identifier, type, symbol), arguments);
        public InvocationExpression Invoke(string identifier, IType? type, ISymbol? symbol, params IExpression[] arguments) => Invoke(Identifier(identifier, type, symbol), arguments);
        public InvocationExpression Invoke(string identifier, IType type, IEnumerable<IExpression> arguments) => Invoke(Identifier(identifier, type), arguments);
        public InvocationExpression Invoke(string identifier, IType type, params IExpression[] arguments) => Invoke(Identifier(identifier, type), arguments);
        public InvocationExpression Invoke(string identifier, ISymbol symbol, IEnumerable<IExpression> arguments) => Invoke(Identifier(identifier, symbol: symbol), arguments);
        public InvocationExpression Invoke(string identifier, ISymbol symbol, params IExpression[] arguments) => Invoke(Identifier(identifier, symbol: symbol), arguments);

        public override MethodDeclarationSyntax SyntaxWithModifiers(Modifier modifier = Modifier.None, Modifier removeModifier = Modifier.None)
            => ToMethodSyntax(modifier, removeModifier);

        public override IEnumerable<ICodeModel> Children()
        {
            foreach (var property in Parameters.Properties) yield return property;
            if (Statements is not null) yield return Statements;
            if (ExpressionBody is not null) yield return ExpressionBody;
            yield return ReturnType;
        }

        // Reflection
        public bool IsFamily => throw new NotImplementedException();
        public bool IsFamilyAndAssembly => throw new NotImplementedException();
        public bool IsFamilyOrAssembly => throw new NotImplementedException();
        public bool IsFinal => throw new NotImplementedException();
        public bool IsGenericMethod => throw new NotImplementedException();
        public bool IsGenericMethodDefinition => throw new NotImplementedException();
        public bool IsHideBySig => throw new NotImplementedException();
        public bool IsPrivate => throw new NotImplementedException();
        public bool IsPublic => throw new NotImplementedException();
        public bool IsSecurityCritical => throw new NotImplementedException();
        public bool IsSecuritySafeCritical => throw new NotImplementedException();
        public bool IsSecurityTransparent => throw new NotImplementedException();
        public bool IsSpecialName => throw new NotImplementedException();
        public bool IsConstructor => throw new NotImplementedException();
        public RuntimeMethodHandle MethodHandle => throw new NotImplementedException();
        public bool IsAssembly => throw new NotImplementedException();
        public bool ContainsGenericParameters => GetGenericArguments().Length > 0;
        public bool IsAbstract => throw new NotImplementedException();
        public MethodImplAttributes MethodImplementationFlags => throw new NotImplementedException();
        public CallingConventions CallingConvention => throw new NotImplementedException();
        new public MethodAttributes Attributes => throw new NotImplementedException();

        public IParameterInfo ReturnParameter => throw new NotImplementedException();

        ITypeInfo IMethodInfo.ReturnType => throw new NotImplementedException();

        public Reflection.ICustomAttributeProvider ReturnTypeCustomAttributes => throw new NotImplementedException();

        public IEnumerable<CustomAttributeData> CustomAttributes => throw new NotImplementedException();

        public ITypeInfo DeclaringType => throw new NotImplementedException();

        public MemberTypes MemberType => throw new NotImplementedException();

        public int MetadataToken => throw new NotImplementedException();

        public Module Module => throw new NotImplementedException();

        public ITypeInfo ReflectedType => throw new NotImplementedException();

        public ExpressionSyntax? ExpressionSyntax => throw new NotImplementedException();

        public IExpression Value => throw new NotImplementedException();

        public ITypeInfo[] GetGenericArguments() => throw new NotImplementedException();
        public MethodBody GetMethodBody() => throw new NotImplementedException();
        public MethodImplAttributes GetMethodImplementationFlags() => throw new NotImplementedException();
        public IParameterInfo[] GetParameters() => throw new NotImplementedException();
        public object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture) => throw new NotImplementedException();
        public object Invoke(object obj, object[] parameters) => throw new NotImplementedException();

        public Delegate CreateDelegate(ITypeInfo delegateType)
        {
            throw new NotImplementedException();
        }

        public Delegate CreateDelegate(ITypeInfo delegateType, object target)
        {
            throw new NotImplementedException();
        }

        public IMethodInfo GetBaseDefinition()
        {
            throw new NotImplementedException();
        }

        public IMethodInfo GetGenericMethodDefinition()
        {
            throw new NotImplementedException();
        }

        public IMethodInfo MakeGenericMethod(params ITypeInfo[] typeArguments)
        {
            throw new NotImplementedException();
        }

        public IList<CustomAttributeData> GetCustomAttributesData()
        {
            throw new NotImplementedException();
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public object[] GetCustomAttributes(ITypeInfo attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public bool IsDefined(ITypeInfo attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public IExpression AccessValue(string identifier, IType? type = null, ISymbol? symbol = null)
        {
            throw new NotImplementedException();
        }

        public IExpression AccessValue(IExpression? instance = null)
        {
            throw new NotImplementedException();
        }

        public ExpressionSyntax? AccessSyntax(IExpression? instance = null)
        {
            throw new NotImplementedException();
        }

        public override CodeModel<MethodDeclarationSyntax> Render(Namespace @namespace)
        {
            throw new NotImplementedException();
        }
    }
}
