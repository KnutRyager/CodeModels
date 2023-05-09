using System;
using System.Collections.Generic;
using System.Linq;
using Common.Files;
using Generator.Models.ProgramModels.GeneratorOptions;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzation.Models.ProgramModels;

public record ProgramModule(
    Namespace TopLevelNamespace,
    List<IProgramModel> Content,
    GeneratorOptions Options)
    : IProgramModel
{
    public static ProgramModule Create(
        Namespace topLevelNamespace,
        IEnumerable<IProgramModel> content,
        GeneratorOptions? options = null)
        => new(topLevelNamespace, content.ToList(), options ?? GeneratorOptions.Default);

    public static ProgramModule Create(
        Namespace topLevelNamespace,
        GeneratorOptions? options = null,
        params IProgramModel[] content)
        => Create(topLevelNamespace, content.ToList(), options);

    public T GetModel<T>(Func<T, bool>? predicate = null) where T : class, IProgramModel
        => (Content.First(x => x is T && predicate is null || predicate!((x as T)!)) as T)!;

    public T? GetModelOrDefault<T>(Func<T, bool>? predicate = null) where T : class, IProgramModel
        => Content.FirstOrDefault(x => x is T && predicate is null || predicate!((x as T)!)) as T;

    public void Add(IProgramModel model)
    {
        Content.Add(model);
    }

    public ICodeModel Render()
    {
        throw new NotImplementedException();
    }

    ICodeModel IProgramModel.Render() => Render();

    public string Code() => Render().Code();

    public ISet<IType> Dependencies(ISet<IType>? set = null)
    {
        throw new NotImplementedException();
    }

    public CSharpSyntaxNode Syntax() => Render().Syntax();

    public IEnumerable<ICodeModel> Children()
    {
        throw new NotImplementedException();
    }

    public Folder ToFolder() => Folder.Create(TopLevelNamespace.Name,
        Content.Select((x, i) => new FileObject(
            $"File{i}",
            (x is IMember member ? member.Render(TopLevelNamespace) : x.Render()).Code(),
            "cs")).ToArray());

    public void Save(string path = "")
        => ToFolder().Save(path);
}
