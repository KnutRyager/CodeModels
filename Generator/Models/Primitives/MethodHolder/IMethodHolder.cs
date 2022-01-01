namespace CodeAnalyzation.Models
{
    public interface IMethodHolder : ICodeModel
    {
        string Name { get; }
        bool IsStatic { get; }
    }
}
