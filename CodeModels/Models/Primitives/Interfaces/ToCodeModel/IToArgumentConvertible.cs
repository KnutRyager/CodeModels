using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public interface IToArgumentConvertible
{
    Argument ToArgument();
}
