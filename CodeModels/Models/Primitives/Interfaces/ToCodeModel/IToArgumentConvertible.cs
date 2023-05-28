using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public interface IToArgumentConvertible : IToArgumentListConvertible
{
    Argument ToArgument();
}
