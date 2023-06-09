﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Factory;
using CodeModels.Models;
using CodeModels.Models.Primitives.Member.Enum;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.AbstractCodeModels.Member;

public record EnumModel(string Identifier, ExpressionCollection Values, Namespace? Namespace, bool IsFlags, bool HasNoneValue)
    : AbstractBaseTypeDeclaration<EnumDeclaration, EnumDeclarationSyntax>(Identifier, new(Values.Values.Select(x => NamedValue((x.LiteralValue() as string)!))), null, Namespace, topLevelModifier: Modifier.Static)
{
    public IEnumerable<IEnumerable<string>>? ValueCategories { get; set; }

    public EnumModel(string identifier, IEnumerable<string>? values = null, Namespace? @namespace = null, bool isFlags = false, bool hasNoneValue = false)
        : this(identifier, new ExpressionCollection(Literals(AddNoneValue(List(values), hasNoneValue))), @namespace, isFlags, hasNoneValue) { }

    public EnumModel(string identifier, IEnumerable<IEnumerable<string>>? valueCategories = null, Namespace? @namespace = null, bool isFlags = false, bool hasNoneValue = false)
        : this(identifier, valueCategories.SelectMany(x => x), @namespace, isFlags, hasNoneValue)
    {
        ValueCategories = valueCategories;
    }

    new public EnumDeclarationSyntax ToEnum() => Values.ToEnum(Name, IsFlags, HasNoneValue);

    private static List<string> AddNoneValue(IEnumerable<string> values, bool hasNoneValue)
    {
        if (hasNoneValue && !values.Contains("None")) values = new string[] { "None" }.Concat(values);
        return values.ToList();
    }

    public override IInstantiatedObject CreateInstance()
    {
        throw new NotImplementedException();
    }

    protected override EnumDeclaration OnToCodeModel(IAbstractCodeModelSettings? settings = null)
        => Enum(Name, GetEnumMembers());
}

//public record EnumFromReflection(Type ReflectedType) : EnumModel(ReflectedType.Name, ReflectedType.GetFields().Select(x => x.Name), Namespace(ReflectedType));
