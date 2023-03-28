namespace CodeAnalyzation.Models.ProgramModels;

public interface IProgramModel : ICodeModel
{
    ICodeModel Render();
}

public interface IProgramModel<T> : IProgramModel where T : ICodeModel
{
    new T Render();
}