using System;
using System.Collections.Generic;
using System.Linq;
using Common.Files;
using Generator.Models.ProgramModels.GeneratorOptions;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models.ProgramModels;

public record ProgramLibrary(
    Namespace TopLevelNamespace,
    List<IMember> Content,
    GeneratorOptions Options)
    : IProgramModel
{
    public static ProgramLibrary Create(
        Namespace topLevelNamespace,
        IEnumerable<IMember> content,
        GeneratorOptions? options = null)
        => new(topLevelNamespace, content.ToList(), options ?? GeneratorOptions.Default);

    public static ProgramLibrary Create(
        Namespace topLevelNamespace,
        GeneratorOptions? options = null,
        params IMember[] content)
        => Create(topLevelNamespace, content, options);

    public T GetModel<T>(Func<T, bool>? predicate = null) where T : class, IProgramModel
     => (Content.First(x => x is T && predicate is null || predicate!((x as T)!)) as T)!;

    public T? GetModelOrDefault<T>(Func<T, bool>? predicate = null) where T : class, IProgramModel
     => Content.FirstOrDefault(x => x is T && predicate is null || predicate!((x as T)!)) as T;

    public void Add(IMember member)
    {
        Content.Add(member);
    }

    public ICodeModel Render()
        => CodeModelFactory.CompilationUnit(Content);

    ICodeModel IProgramModel.Render() => Render();

    public string Code() => Render().Code();

    public ISet<IType> Dependencies(ISet<IType>? set = null)
    {
        throw new NotImplementedException();
    }

    public CSharpSyntaxNode Syntax() => Render().Syntax();

    public IEnumerable<ICodeModel> Children()
        => Content;

    public Folder ToFolder() => Folder.Create("ProgramModel",
        Content.Select((x, i) => new FileObject($"File{i}", 
            CodeModelFactory.CompilationUnit(
                members: new List<IMember>() { x },
                usings: new List<UsingDirective>(),
                externs : new List<ExternAliasDirective>()
                ).Code(), "cs")).ToArray());

    public void Save(string path = "")
        => ToFolder().Save(path);
}
