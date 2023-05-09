using System.Collections.Generic;
using System.Linq;
using CodeModels.Extensions;
using CodeModels.Models;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static CodeModels.Models.CodeModelFactory;

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

        var staticClass = new StaticClass("ModelDependencies", @namespace: Namespace("Dependencies"));
        var dependencyDictionaries = dependenciesWithFullPaths.Select(x => new ExpressionMap(x.Properties.Select(
            y => new ExpressionsMap(Literal(y.Member.Name),
            Literals(y.Dependencies), valueType: Type("string", isMulti: true), multiValues: true)), x.Class.Name));
        foreach (var dict in dependencyDictionaries)
        {
            staticClass.AddProperty(dict.ToProperty());
        }
        var masterDependencyDictionary = new ExpressionMap(dependenciesWithFullPaths.Select(
            x => new ExpressionsMap(Literal(x.Class.Name),
            new PropertyExpression(staticClass.Properties.Properties.First(y => y.Name == x.Class.Name)))), "Deps");
        staticClass.AddProperty(masterDependencyDictionary.ToProperty());

        return staticClass.ToClass();
    }
}
