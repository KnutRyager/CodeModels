using System.Collections.Generic;
using System.Linq;
using CodeAnalyzation.Models;
using Common.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalyzation.Generation
{
    public static class DependencyGeneration
    {
        public static ClassDeclarationSyntax GenerateDependencies(IEnumerable<SyntaxTree> trees, SemanticModel model)
        {
            var modelClasses = trees.GetModelClasses();
            var dependencies = modelClasses.GetFullDependencies(model);
            var dependenciesWithFullPaths = dependencies.Select(x => (Class: x.Key, Properties: x.Value.Select(x => (x.Member, Dependencies: x.Dependencies.SelectMany(y => y.Transform(z => z.Name)
                .TransformByTransformedParent<string>((node, parent) => $"{StringUtil.FilterJoin(parent, node)}").ToList()).Distinct())))).ToList();

            var staticClass = new StaticClass("ModelDependencies", @namespace: new("Dependencies"));
            var dependencyDictionaries = dependenciesWithFullPaths.Select(x => new ValueDictionary(x.Properties.Select(
                y => new ValueCollectionWithKey(Value.FromValue(y.Member.Name),
                y.Dependencies.Select(z => Value.FromValue(z)), valueType: new TType("string", isMulti: true), multiValues: true)), x.Class.Name));
            foreach (var dict in dependencyDictionaries)
            {
                staticClass.AddProperty(dict.ToProperty());
            }
            var masterDependencyDictionary = new ValueDictionary(dependenciesWithFullPaths.Select(
                x => new ValueCollectionWithKey(Value.FromValue(x.Class.Name),
                new Value(staticClass.Properties.Properties.First(y => y.Name == x.Class.Name)))), "Deps");
            staticClass.AddProperty(masterDependencyDictionary.ToProperty());

            return staticClass.ToClass();
        }
    }
}