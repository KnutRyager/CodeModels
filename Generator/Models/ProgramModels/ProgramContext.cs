namespace CodeAnalyzation.Models.ProgramModels;

public interface IProgramContext
{
    IExpression GetSingleton(IType type);
}

public record ProgramContext() : IProgramContext
{
    public IExpression GetSingleton(IType type)
    {
        throw new System.NotImplementedException();
    }
}
