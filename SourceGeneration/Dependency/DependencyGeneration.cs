using System.Collections.Generic;
using System.Linq;
using CodeModels.AbstractCodeModels.Collection;
using CodeModels.Extensions;
using CodeModels.Models;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Factory.AbstractCodeModelFactory;
using static CodeModels.Factory.CodeModelFactory;

namespace CodeModels.Generation;

public static class DependencyGeneration
{
    public static ClassDeclarationSyntax GenerateDependencies(IEnumerable<SyntaxTree> trees, Microsoft.CodeAnalysis.Compilation compilation)
        => GenerateDependencies(trees, compilation.GetSemanticModel(trees.First()));
    public static ClassDeclarationSyntax GenerateDependencies(IEnumerable<SyntaxTree> trees, SemanticModel model)
    {
        var modelClasses = trees.GetModelClasses();
        var dependencies = modelClasses.GetFullDependencies(model);
        var dependenciesWithFullPaths = dependencies.Select(x => (Class: x.Key, Properties: x.Value.Select(x => (x.Member, Dependencies: x.Dependencies.SelectMany(y => y.Transform(z => z.Name)
            .TransformByTransformedParent<string>((node, parent) => $"{StringUtil.FilterJoin(parent, node)}").ToList()).Distinct())))).ToList();

        var staticClass = StaticClass("ModelDependencies", @namespace: Namespace("Dependencies"));
        var dependencyDictionaries = dependenciesWithFullPaths.Select(x => ExpressionMap(x.Properties.Select(
            y => ExpressionsMap(Literal(y.Member.Name),
            Literals(y.Dependencies), valueType: Type<string[]>(), multiValues: true)), x.Class.Name));
        foreach (var dict in dependencyDictionaries)
        {
            staticClass.AddProperty(dict.ToNamedValue());
        }
        var masterDependencyDictionary = ExpressionMap(dependenciesWithFullPaths.Select(
            x => ExpressionsMap(Literal(x.Class.Name),
            Expression(staticClass.Properties.Properties.First(y => y.Name == x.Class.Name)))), "Deps");
        staticClass.AddProperty(masterDependencyDictionary.ToNamedValue());

        return staticClass.ToClass();
    }
}
