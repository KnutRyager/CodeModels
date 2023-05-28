using CodeModels.Models.Primitives.Member;

namespace CodeModels.Models;

public interface IToArgumentListConvertible
{
    ArgumentList ToArgumentList();
}
