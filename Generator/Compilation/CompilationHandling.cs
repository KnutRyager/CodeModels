using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static CodeAnalyzation.SyntaxNodeExtensions;

namespace CodeAnalyzation.Compilation;

public static class CompilationHandling
{
    private static void SetSemanticModel(IEnumerable<SyntaxTree> trees, string? key = null)
    {
        var Mscorlib = GetReference<object>();
        var linqLib = GetReference(typeof(Enumerable));
        var collectionsLib = GetReference(typeof(List<>));
        var compilationWithModel = CSharpCompilation.Create("MyCompilation",
            syntaxTrees: trees, references: new[] { Mscorlib, linqLib, collectionsLib });
        //Note that we must specify the tree for which we want the model.
        //Each tree has its own semantic model
        SetCompilation(compilationWithModel, trees, key);
    }

    public static T StoreSyntax<T>(T node, string? key = null) where T : CSharpSyntaxNode
    {
        StoreSyntaxTree((node.SyntaxTree as CSharpSyntaxTree)!, key);
        return node;
    }

    public static CSharpSyntaxTree StoreSyntaxTree(CSharpSyntaxTree tree, string? key = null)
    {
        SetSemanticModel(new[] { tree }, key ?? tree.ToString());
        return tree;
    }

    private static PortableExecutableReference GetReference<T>() => GetReference(typeof(T));
    private static PortableExecutableReference GetReference(Type type) => MetadataReference.CreateFromFile(type.Assembly.Location);
}
