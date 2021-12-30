namespace CodeAnalyzation.Models
{
    public interface IMethodHolder
    {
        string Name { get; }
        bool IsStatic { get; }
    }
}
