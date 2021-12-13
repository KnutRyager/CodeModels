﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation
{
    public class CompilationContext
    {
        private CSharpCompilation? _compilation;
        public CSharpCompilation Compilation => _compilation ?? throw new Exception("_compilation not initialized.");

        private SemanticModel? _semanticModel;
        public SemanticModel SemanticModel => _semanticModel ?? throw new Exception("_semanticModel not initialized.");

        public void SetCompilation(CSharpCompilation compilation, IEnumerable<SyntaxTree> trees)
        {
            _compilation = compilation;
            _semanticModel = GetSemanticModel(trees.FirstOrDefault());
        }

        public void SetSemanticModel(SyntaxTree tree)
            => _semanticModel = GetSemanticModel(tree);

        public void SetSemanticModel(int treeIndex)
            => _semanticModel = GetSemanticModel(Compilation.SyntaxTrees[treeIndex]);

        public SemanticModel GetSemanticModel(SyntaxTree? tree = null) => (tree == null)! ? SemanticModel : Compilation.GetSemanticModel(tree);
    }
}